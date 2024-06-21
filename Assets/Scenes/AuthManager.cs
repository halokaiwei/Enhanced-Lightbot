using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase;
using Firebase.Auth;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;  

public class AuthManager : MonoBehaviour
{
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

    [Header("Login")]
    public InputField emailLoginField;
    public InputField passwordLoginField;
    public InputField forgetPasswordEmail;
    public TMP_Text resetText;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    [Header("Register")]
    public InputField usernameRegisterField;
    public InputField emailRegisterField;
    public InputField passwordRegisterField;
    public InputField passwordVerifyRegisterField;
    public TMP_Text warningRegisterText;

    public InitializeUserData initializeuserdata;

    void Start()
    {
        StartCoroutine(InitializeFirebaseCoroutine());
    }

    private IEnumerator InitializeFirebaseCoroutine()
    {
        Debug.Log("Initializing Firebase...");
        yield return FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available) {
                InitializeFirebase();
            } else {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    protected void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        Debug.Log("Firebase Auth setup complete"); 
    }

    public void ClearLoginFields() {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }

    public void ClearRegisterFields() {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text =  "";
        passwordVerifyRegisterField.text = "";
    }

    public void LoginButton() {
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    public void RegisterButton() {
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    public void AnonymousLoginButton() {
        StartCoroutine(AnonymousLogin());
    }

    public void ForgetPasswordButton() {
        StartCoroutine(ForgetPasswordSubmit(forgetPasswordEmail.text));
    }

    public void SignOutButton() {
        auth.SignOut();
        UIManager.instance.LoginScreen();
        ClearRegisterFields();
        ClearLoginFields();
    }

    private IEnumerator Login(string _email, string _password) {
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email,_password);
        yield return new WaitUntil(() => LoginTask.IsCompleted);
        Debug.Log(LoginTask.IsCompleted);
        if (LoginTask.Exception != null) {
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            string message = "Login Failed";
            switch(errorCode) {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Email or password incorrect";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else {
            AuthResult authResult = LoginTask.Result;
            User = authResult.User;
            if (User != null)
            {
                Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
                warningLoginText.text = "";
                confirmLoginText.text = "Logged In";
                yield return new WaitForSeconds(2);
                PlayerPrefs.SetString("Type", "signin");
                PlayerPrefs.SetString("Username", User.DisplayName);
                PlayerPrefs.SetString("Email", User.Email);
                PlayerPrefs.Save(); 
                SceneManager.LoadScene("Signed-in");
                confirmLoginText.text = "";
                ClearLoginFields();
                ClearRegisterFields();
            }
            else
            {
                Debug.LogError("User object is null after successful login. Check Firebase authentication settings.");
            }
        }
    }

    private IEnumerator Register(string _email,string _password, string _username) {
        LevelTime levelTime = FindObjectOfType<LevelTime>();
        if (_username == "") {
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordVerifyRegisterField.text) {
            warningRegisterText.text = "Passwords do not match";
        }
        else {
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email,_password);
            yield return new WaitUntil(()=>RegisterTask.IsCompleted);
            if (RegisterTask.Exception != null) {
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed";
                switch(errorCode) {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email had already in use";
                        break;

                }
                warningRegisterText.text = message;
            }
            else {
                AuthResult authResult = RegisterTask.Result;
                User = authResult.User;
                Debug.Log(authResult);

                if (User != null)
                {
                    UserProfile profile = new UserProfile { DisplayName = _username };
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    yield return new WaitUntil(() => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        Debug.LogWarning($"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username set failed";
                    }
                    else
                    {
                        warningRegisterText.text = "Sign Up Successfully";
                        yield return new WaitForSeconds(2);
                        UIManager.instance.LoginScreen();
                        initializeuserdata.InitializeuserData(User.DisplayName);
                        ClearLoginFields();
                        ClearRegisterFields();
                    }
                }
                else
                {//checking
                    Debug.Log("User object is null after successful registration. Check Firebase authentication settings.");
                }
            }
        }
    }

    private IEnumerator AnonymousLogin() {
        var anoTask = auth.SignInAnonymouslyAsync();
        yield return new WaitUntil(() => anoTask.IsCompleted);
        if (anoTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {anoTask.Exception}");
            FirebaseException firebaseEx = anoTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Anonymous Login Failed";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                // Add more cases as needed
                default:
                    message = "Unknown error during anonymous login";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            AuthResult result = anoTask.Result;
            UIManager.instance.ClearScreen();
            PlayerPrefs.SetString("Type", "guests");
            PlayerPrefs.Save(); 
            Debug.LogFormat("User signed in anonymously successfully: {0} ({1})", 
            result.User.DisplayName, result.User.UserId);
            SceneManager.LoadScene("Signed-in");
        }
    }

private IEnumerator ForgetPasswordSubmit(string forgetPasswordEmail)
{
    var resetTask = auth.SendPasswordResetEmailAsync(forgetPasswordEmail);
    yield return new WaitUntil(() => resetTask.IsCompleted);
    if (resetTask.Exception != null)
    {
        Debug.LogWarning($"Failed to send reset password with {resetTask.Exception}");
        FirebaseException firebaseEx = resetTask.Exception.GetBaseException() as FirebaseException;
        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
        string message = "Password reset failed";
        switch (errorCode)
        {
            case AuthError.MissingEmail:
                message = "Missing Email";
                break;
            case AuthError.InvalidEmail:
                message = "Invalid Email";
                break;
            default:
                message = "This email has no registered";
                break;
        }
        resetText.text = message;
    }
    else
    {
        resetText.text = "Password reset email sent successfully.";
        UIManager.instance.LoginScreen();
    }
}
}
