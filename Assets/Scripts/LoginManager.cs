using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    
    public TMP_InputField usernameInputField_TMP; 
    public TMP_InputField passwordInputField_TMP;
    public Button loginButton_TMP; 
    public TMP_Text errorMessageText_TMP; 

    // Static login credentials
    private string correctUsername = "Metabookxr";
    private string correctPassword = "12345";

    void Start()
    {
        // Add a listener to the login button
        loginButton_TMP.onClick.AddListener(CheckLogin);

        // Clear the error message at the start
        errorMessageText_TMP.text = "";
         // Set error message color to red
    }

    void CheckLogin()
    {
        string enteredUsername = usernameInputField_TMP.text;
        string enteredPassword = passwordInputField_TMP.text;

        // Check if the entered credentials match the static credentials
        if (enteredUsername == correctUsername && enteredPassword == correctPassword)
        {
            errorMessageText_TMP.color = Color.green;
            Debug.Log("Login Successful!");
            errorMessageText_TMP.text = "Login Successful!";
            SceneManager.LoadScene("MenuPanel");
        }
        else
        {
            errorMessageText_TMP.color = Color.red;
            Debug.Log("Login Failed. Incorrect username or password.");
            DisplayErrorMessage("Incorrect username or password."); // Display error message
        }
    }

    void DisplayErrorMessage(string message)
    {
        errorMessageText_TMP.text = message; // Set the error message text
        StartCoroutine(ClearErrorMessageAfterDelay(3f)); // Clear the message after 3 seconds
    }

    IEnumerator ClearErrorMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        errorMessageText_TMP.text = ""; // Clear the error message
    }
}