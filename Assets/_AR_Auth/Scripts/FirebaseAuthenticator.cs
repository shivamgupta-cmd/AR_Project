using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Required for loading new scenes
using TMPro;

// Import Firebase namespaces
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;

public class FirebaseAuthenticator : MonoBehaviour
{
    // Set the maximum number of logins allowed per day
    private const int MAX_LOGINS_PER_DAY = 5;

    [Tooltip("The name of the scene to load after a successful login.")]
    public string mainSceneName = "MainScene"; // Name of your main gameplay scene

    // --- Public UI References ---
    [Header("UI Panels")]
    public GameObject loginPanel;
    public GameObject signupPanel;

    [Header("Login Panel UI")]
    public TMP_InputField loginEmailInputField;
    public TMP_InputField loginPasswordInputField;
    public Button loginButton;
    public Button switchToSignupButton;
    public TMP_Text loginErrorText;

    [Header("Signup Panel UI")]
    public TMP_InputField signupEmailInputField;
    public TMP_InputField signupPasswordInputField;
    public TMP_InputField signupConfirmPasswordInputField;
    public Button signupButton;
    public Button switchToLoginButton;
    public TMP_Text signupErrorText;

    // --- Firebase Variables ---
    protected FirebaseAuth auth;
    protected FirebaseFirestore db;
    protected Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    private FirebaseUser currentUser;

    // --- Unity Lifecycle Methods ---

    void Start()
    {
        Debug.Log("FirebaseAuthenticator Start(). Checking Firebase dependencies without fix popup...");
        FirebaseApp.CheckDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                loginErrorText.text = "Could not connect to Firebase.";
                signupErrorText.text = "Could not connect to Firebase.";
            }
        });
    }

    void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
            auth = null;
        }
    }

    // --- Firebase Initialization ---

    void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth...");
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null); // Initializes currentUser but doesn't trigger login

        Debug.Log("Adding UI Button Listeners...");
        loginButton.onClick.AddListener(OnLoginButtonPressed);
        signupButton.onClick.AddListener(OnSignupButtonPressed);
        switchToSignupButton.onClick.AddListener(ShowSignupPanel);
        switchToLoginButton.onClick.AddListener(() => ShowLoginPanel());

        Debug.Log("Firebase Initialized Successfully.");
        //ShowLoginPanel(); // **Crucially, always start on the login panel**
    }

    // --- Core Authentication Logic ---

    private async void SignInAsync(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            loginErrorText.text = "Please enter email and password.";
            return;
        }

        Debug.Log($"Login button pressed. Attempting to sign in with email: {email}");
        loginErrorText.text = "Logging in...";

        try
        {
            // 1. Await the sign-in task from Firebase Auth
            var userCredential = await auth.SignInWithEmailAndPasswordAsync(email, password);
            Debug.Log($"Firebase Auth successful for {userCredential.User.Email}.");

            // 2. **CHANGE**: Manually call the approval check AFTER successful sign-in
            await CheckUserApprovalAndLoadSceneAsync(userCredential.User);
        }
        catch (Exception e)
        {
            Debug.LogError($"Sign-in flow failed with error: {e}");
            loginErrorText.text = $" {e.InnerException?.Message ?? e.Message}";
        }
    }

    // **CHANGED**: This method now returns a Task and is called directly from SignInAsync
    private async Task CheckUserApprovalAndLoadSceneAsync(FirebaseUser user)
    {
        var userDocRef = db.Collection("users").Document(user.UserId);
        Debug.Log($"Checking approval status for user {user.UserId} at path: {userDocRef.Path}");

        var snapshot = await userDocRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            var userData = snapshot.ToDictionary();

            if (userData.TryGetValue("isApproved", out var isApprovedObj) && isApprovedObj is bool isApproved && isApproved)
            {
                Debug.Log("User is APPROVED. Checking login count...");

                string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
                long todaysLogins = 0;
                if (userData.TryGetValue("loginHistory", out var historyObj) && historyObj is Dictionary<string, object> historyMap && historyMap.TryGetValue(today, out var countObj))
                {
                    todaysLogins = (long)countObj;
                }

                if (todaysLogins >= MAX_LOGINS_PER_DAY)
                {
                    Debug.LogWarning("User has reached the daily login limit. Forcing logout.");
                    auth.SignOut();
                    // Throw an exception to be caught by SignInAsync's catch block
                    throw new Exception("You have reached your daily login limit.");
                }

                Debug.Log("All checks passed. Loading main scene.");
                await TrackLoginAsync(user);
                SceneManager.LoadScene(mainSceneName);
            }
            else
            {
                Debug.LogWarning("User is NOT APPROVED. Forcing logout.");
                auth.SignOut();
                throw new Exception("Your account is still pending approval.");
            }
        }
        else
        {
            Debug.LogError("CRITICAL: User is authenticated, but their document does not exist.");
            auth.SignOut();
            throw new Exception("User data not found. Please contact support.");
        }
    }

    // **CHANGED**: This no longer triggers the login flow. It only manages the currentUser state.
    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != currentUser)
        {
            bool signedIn = auth.CurrentUser != null;
            if (!signedIn && currentUser != null)
            {
                // This block runs when auth.SignOut() is called
                Debug.Log("User has been signed out.");
            }

            currentUser = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log($"Auth state changed: User is signed in as {currentUser.Email}");
                // NOTE: We no longer call the approval check from here to prevent auto-login.
            }
        }
    }

    // --- Unchanged Methods from previous version ---
    #region Unchanged Methods
    private async void SignUpAsync(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            signupErrorText.text = "Please enter email and password.";
            return;
        }

        Debug.Log($"Attempting to create user with email: {email}");
        signupErrorText.text = "Creating account...";

        try
        {
            var userCredential = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            Debug.Log($"Firebase Auth user created successfully: {userCredential.User.UserId}");
            signupErrorText.text = "";

            await CreateUserDocumentAsync(userCredential.User);

            Debug.Log("Sign-up successful. Forcing sign-out and redirecting to login screen.");
            auth.SignOut();

            ShowLoginPanel("Account created. Waiting for administrator approval.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Sign-up failed with error: {e}");
            signupErrorText.text = $"{e.InnerException?.Message ?? e.Message}";
        }
    }

    private async Task CreateUserDocumentAsync(FirebaseUser user)
    {
        var userDocRef = db.Collection("users").Document(user.UserId);
        var userData = new Dictionary<string, object>
        {
            { "email", user.Email },
            { "signupDate", FieldValue.ServerTimestamp },
            { "isApproved", false }
        };
        await userDocRef.SetAsync(userData);
    }

    private async Task TrackLoginAsync(FirebaseUser user)
    {
        if (user == null) return;
        var userDocRef = db.Collection("users").Document(user.UserId);
        string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var userData = new Dictionary<string, object>
        {
            { "lastLogin", FieldValue.ServerTimestamp },
            { "loginHistory", new Dictionary<string, object> { { today, FieldValue.Increment(1) } } }
        };
        await userDocRef.SetAsync(userData, SetOptions.MergeAll);
    }

    private void ShowLoginPanel(string message = null)
    {
        ClearUIState();
        loginPanel.SetActive(true);
        if (!string.IsNullOrEmpty(message))
        {
            loginErrorText.text = message;
        }
    }

    private void ShowSignupPanel()
    {
        ClearUIState();
        signupPanel.SetActive(true);
    }

    private void OnLoginButtonPressed()
    {
        SignInAsync(loginEmailInputField.text, loginPasswordInputField.text);
    }

    private void OnSignupButtonPressed()
    {
        if (signupPasswordInputField.text != signupConfirmPasswordInputField.text)
        {
            signupErrorText.text = "Passwords do not match.";
            return;
        }
        SignUpAsync(signupEmailInputField.text, signupPasswordInputField.text);
    }

    private void ClearUIState()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        loginErrorText.text = "";
        signupErrorText.text = "";
    }
    #endregion
}
