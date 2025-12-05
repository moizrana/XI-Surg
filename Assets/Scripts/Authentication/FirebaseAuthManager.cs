// using System;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;

// namespace Meducator.Authentication
// {
//     public class FirebaseAuthManager : MonoBehaviour
//     {
//         [Header("Firebase Configuration")]
//         public string firebaseApiKey = "YOUR_FIREBASE_API_KEY";
//         public string firebaseDatabaseUrl = "https://meducator-c188d-default-rtdb.firebaseio.com/";

//         [Header("UI References")]
//         public InputField emailInput;
//         public InputField passwordInput;
//         public Button loginButton;
//         public Button registerButton;
//         public Text statusText;
//         public GameObject loadingIndicator;

//         [Header("Scene Settings")]
//         public string mainSceneName = "SampleScene";

//         private bool isLoading = false;

//         // Events
//         public static event Action<bool> OnAuthenticationChanged;
//         public static event Action<string> OnAuthStatusChanged;

//         private void Start()
//         {
//             InitializeUI();
//             CheckAuthenticationStatus();
//         }

//         private void InitializeUI()
//         {
//             if (loginButton != null)
//                 loginButton.onClick.AddListener(LoginUser);

//             if (registerButton != null)
//                 registerButton.onClick.AddListener(OpenRegistrationWebsite);

//             SetLoadingState(false);
//             UpdateStatusText("Enter your credentials to continue", Color.white);
//         }

//         private void CheckAuthenticationStatus()
//         {
//             // Check if user is already authenticated
//             string savedToken = PlayerPrefs.GetString("FirebaseAuthToken", "");
//             if (!string.IsNullOrEmpty(savedToken))
//             {
//                 ValidateStoredToken(savedToken);
//             }
//         }

//         private async void ValidateStoredToken(string token)
//         {
//             try
//             {
//                 SetLoadingState(true);
//                 UpdateStatusText("Validating credentials...", Color.yellow);

//                 // Simulate token validation (replace with actual Firebase token validation)
//                 await Task.Delay(1000);

//                 if (IsTokenValid(token))
//                 {
//                     UpdateStatusText("Login successful!", Color.green);
//                     OnAuthenticationChanged?.Invoke(true);
//                     await Task.Delay(1000);
//                     LoadMainScene();
//                 }
//                 else
//                 {
//                     PlayerPrefs.DeleteKey("FirebaseAuthToken");
//                     UpdateStatusText("Session expired. Please login again.", Color.red);
//                     SetLoadingState(false);
//                 }
//             }
//             catch (System.Exception e)
//             {
//                 Debug.LogError($"Token validation error: {e.Message}");
//                 UpdateStatusText("Authentication error. Please try again.", Color.red);
//                 SetLoadingState(false);
//             }
//         }

// private async void LoginUser()
//         {
//             if (isLoading) return;

//             string email = emailInput?.text ?? emailTMPInput?.text ?? "";
//             string password = passwordInput?.text ?? passwordTMPInput?.text ?? "";

//             if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
//             {
//                 UpdateStatusText("Please enter both email and password", Color.red);
//                 return;
//             }

//             if (!IsValidEmail(email))
//             {
//                 UpdateStatusText("Please enter a valid email address", Color.red);
//                 return;
//             }

//             await PerformLogin(email, password);
//         }

//         private async Task PerformLogin(string email, string password)
//         {
//             try
//             {
//                 SetLoadingState(true);
//                 UpdateStatusText("Signing in...", Color.yellow);

//                 // Simulate Firebase authentication (replace with actual Firebase SDK calls)
//                 await Task.Delay(2000);

//                 bool loginSuccess = await AuthenticateWithFirebase(email, password);

//                 if (loginSuccess)
//                 {
//                     string token = GenerateMockToken(email);
//                     PlayerPrefs.SetString("FirebaseAuthToken", token);
//                     PlayerPrefs.SetString("UserEmail", email);

//                     UpdateStatusText("Login successful!", Color.green);
//                     OnAuthenticationChanged?.Invoke(true);
//                     OnAuthStatusChanged?.Invoke($"Welcome back, {email}");

//                     await Task.Delay(1500);
//                     LoadMainScene();
//                 }
//                 else
//                 {
//                     UpdateStatusText("Invalid credentials. Please try again.", Color.red);
//                     SetLoadingState(false);
//                 }
//             }
//             catch (System.Exception e)
//             {
//                 Debug.LogError($"Login error: {e.Message}");
//                 UpdateStatusText("Authentication failed. Please check your connection.", Color.red);
//                 SetLoadingState(false);
//             }
//         }

//         private async Task<bool> AuthenticateWithFirebase(string email, string password)
//         {
//             // This is a mock implementation
//             // Replace with actual Firebase authentication

//             // Simulate network delay
//             await Task.Delay(1500);

//             // Mock validation - replace with real Firebase authentication
//             // For now, accept any email/password combination for testing
//             return !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password);

//             // TODO: Replace with actual Firebase authentication:
//             // var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
//             // var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
//             // return result.User != null;
//         }

//         private string GenerateMockToken(string email)
//         {
//             // Generate a mock token for demonstration
//             // Replace with actual Firebase ID token
//             return $"mock_token_{email}_{DateTime.Now.Ticks}";
//         }

//         private bool IsTokenValid(string token)
//         {
//             // Mock token validation
//             // Replace with actual Firebase token validation
//             return token.StartsWith("mock_token_");
//         }

//         private void LoadMainScene()
//         {
//             SceneManager.LoadScene(mainSceneName);
//         }

//         private void OpenRegistrationWebsite()
//         {
//             UpdateStatusText("Opening registration website...", Color.yellow);

//             // Open the registration website in the default browser
//             Application.OpenURL("https://meducator.vercel.app/auth/login");

//             // Reset status after a delay
//             Invoke(nameof(ResetStatusText), 2f);
//         }

//         private void ResetStatusText()
//         {
//             UpdateStatusText("Please register on the website and return to login", Color.white);
//         }

//         private void SetLoadingState(bool loading)
//         {
//             isLoading = loading;

//             if (loginButton != null)
//                 loginButton.interactable = !loading;

//             if (registerButton != null)
//                 registerButton.interactable = !loading;

//             if (emailInput != null)
//                 emailInput.interactable = !loading;

//             if (passwordInput != null)
//                 passwordInput.interactable = !loading;

//             if (loadingIndicator != null)
//                 loadingIndicator.SetActive(loading);
//         }

//         private void UpdateStatusText(string message, Color color)
//         {
//             if (statusText != null)
//             {
//                 statusText.text = message;
//                 statusText.color = color;
//             }

//             OnAuthStatusChanged?.Invoke(message);
//         }

//         private bool IsValidEmail(string email)
//         {
//             try
//             {
//                 var addr = new System.Net.Mail.MailAddress(email);
//                 return addr.Address == email;
//             }
//             catch
//             {
//                 return false;
//             }
//         }

//         public void Logout()
//         {
//             PlayerPrefs.DeleteKey("FirebaseAuthToken");
//             PlayerPrefs.DeleteKey("UserEmail");
//             OnAuthenticationChanged?.Invoke(false);
//             UpdateStatusText("Logged out successfully", Color.green);
//         }

//         public string GetCurrentUserEmail()
//         {
//             return PlayerPrefs.GetString("UserEmail", "");
//         }

//         public bool IsUserAuthenticated()
//         {
//             string token = PlayerPrefs.GetString("FirebaseAuthToken", "");
//             return !string.IsNullOrEmpty(token);
//         }


//         [Header("TMP UI References")]
//         public TMPro.TMP_InputField emailTMPInput;
//         public TMPro.TMP_InputField passwordTMPInput;
// }
// }


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Meducator.Authentication
{
    public class FirebaseAuthManager : MonoBehaviour
    {
        [Header("Firebase Configuration")]
        public string firebaseDatabaseUrl = "https://meducator-c188d-default-rtdb.firebaseio.com/";
        public string firebaseWebApiKey = "YOUR_FIREBASE_WEB_API_KEY"; // Get from Firebase Console

        [Header("UI References")]
        public InputField emailInput;
        public InputField passwordInput;
        public Button loginButton;
        public Button registerWebButton;
        public Text statusText;
        public GameObject loadingIndicator;

        [Header("Scene Settings")]
        public string mainSceneName = "SelectionScene";
        public string registrationWebUrl = "https://meducator.vercel.app/auth/login";

        private bool isLoading = false;

        // Events
        public static event Action<bool> OnAuthenticationChanged;
        public static event Action<string> OnAuthStatusChanged;

        // Firebase REST API endpoints
        private const string FIREBASE_AUTH_URL = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=";

        [System.Serializable]
        private class FirebaseLoginRequest
        {
            public string email;
            public string password;
            public bool returnSecureToken = true;
        }

        [System.Serializable]
        private class FirebaseLoginResponse
        {
            public string idToken;
            public string email;
            public string refreshToken;
            public string expiresIn;
            public string localId;
        }

        private void Start()
        {
            InitializeUI();
            CheckAuthenticationStatus();
        }

        private void InitializeUI()
        {
            if (loginButton != null)
                loginButton.onClick.AddListener(LoginUser);

            if (registerWebButton != null)
                registerWebButton.onClick.AddListener(OpenRegistrationWebsite);

            SetLoadingState(false);
            UpdateStatusText("Welcome to Meducator", Color.white);
        }

        private void CheckAuthenticationStatus()
        {
            string savedToken = PlayerPrefs.GetString("FirebaseAuthToken", "");
            string savedEmail = PlayerPrefs.GetString("UserEmail", "");

            if (!string.IsNullOrEmpty(savedToken) && !string.IsNullOrEmpty(savedEmail))
            {
                UpdateStatusText("Restoring session...", Color.yellow);
                StartCoroutine(ValidateAndLoadMainScene(savedToken, savedEmail));
            }
        }

        private IEnumerator ValidateAndLoadMainScene(string token, string email)
        {
            yield return new WaitForSeconds(0.5f);

            // Check if user still exists in Firebase Database
            using (UnityWebRequest request = UnityWebRequest.Get($"{firebaseDatabaseUrl}/users.json?auth={token}"))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    UpdateStatusText($"Welcome back, {email}!", Color.green);
                    OnAuthenticationChanged?.Invoke(true);
                    yield return new WaitForSeconds(1f);
                    LoadMainScene();
                }
                else
                {
                    // Session expired, clear data
                    ClearAuthData();
                    UpdateStatusText("Session expired. Please login again.", Color.red);
                }
            }
        }

        public void LoginUser()
        {
            if (isLoading) return;

            string email = emailInput?.text ?? "";
            string password = passwordInput?.text ?? "";

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                UpdateStatusText("Please enter email and password", Color.red);
                return;
            }

            if (!IsValidEmail(email))
            {
                UpdateStatusText("Please enter a valid email address", Color.red);
                return;
            }

            StartCoroutine(PerformFirebaseLogin(email, password));
        }

        private IEnumerator PerformFirebaseLogin(string email, string password)
        {
            SetLoadingState(true);
            UpdateStatusText("Signing in...", Color.yellow);

            // Step 1: Authenticate with Firebase Authentication
            FirebaseLoginRequest loginData = new FirebaseLoginRequest
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            string jsonData = JsonUtility.ToJson(loginData);

            using (UnityWebRequest authRequest = new UnityWebRequest(FIREBASE_AUTH_URL + firebaseWebApiKey, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                authRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                authRequest.downloadHandler = new DownloadHandlerBuffer();
                authRequest.SetRequestHeader("Content-Type", "application/json");

                yield return authRequest.SendWebRequest();

                if (authRequest.result == UnityWebRequest.Result.Success)
                {
                    FirebaseLoginResponse response = null;
                    try
                    {
                        response = JsonUtility.FromJson<FirebaseLoginResponse>(authRequest.downloadHandler.text);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error parsing login response: {e.Message}");
                        UpdateStatusText("Login error. Please try again.", Color.red);
                        SetLoadingState(false);
                        yield break;
                    }

                    if (response != null)
                    {
                        // Step 2: Verify user exists in Realtime Database
                        yield return StartCoroutine(VerifyUserInDatabase(response.idToken, response.localId, response.email));
                    }
                }
                else
                {
                    string errorMessage = "Invalid email or password";

                    try
                    {
                        var errorResponse = JsonUtility.FromJson<FirebaseErrorResponse>(authRequest.downloadHandler.text);
                        if (errorResponse != null && errorResponse.error != null)
                        {
                            if (errorResponse.error.message.Contains("INVALID_PASSWORD"))
                                errorMessage = "Invalid password";
                            else if (errorResponse.error.message.Contains("EMAIL_NOT_FOUND"))
                                errorMessage = "Email not found. Please register first.";
                            else if (errorResponse.error.message.Contains("USER_DISABLED"))
                                errorMessage = "Account has been disabled";
                        }
                    }
                    catch { }

                    Debug.LogError($"Login failed: {authRequest.downloadHandler.text}");
                    UpdateStatusText(errorMessage, Color.red);
                    SetLoadingState(false);
                }
            }
        }

        private IEnumerator VerifyUserInDatabase(string token, string userId, string email)
        {
            // Check if user exists in Firebase Realtime Database
            using (UnityWebRequest dbRequest = UnityWebRequest.Get($"{firebaseDatabaseUrl}/users/{userId}.json?auth={token}"))
            {
                yield return dbRequest.SendWebRequest();

                if (dbRequest.result == UnityWebRequest.Result.Success)
                {
                    string userData = dbRequest.downloadHandler.text;

                    if (userData != "null" && !string.IsNullOrEmpty(userData))
                    {
                        // User exists in database, proceed with login
                        SaveAuthData(token, email, userId, userData);

                        UpdateStatusText("Login successful!", Color.green);
                        OnAuthenticationChanged?.Invoke(true);
                        OnAuthStatusChanged?.Invoke($"Welcome back!");

                        yield return new WaitForSeconds(1.5f);
                        LoadMainScene();
                    }
                    else
                    {
                        UpdateStatusText("User not found in database. Please register on website.", Color.red);
                        SetLoadingState(false);
                    }
                }
                else
                {
                    UpdateStatusText("Error verifying user. Please try again.", Color.red);
                    SetLoadingState(false);
                }
            }
        }

        private void SaveAuthData(string token, string email, string userId, string userData)
        {
            PlayerPrefs.SetString("FirebaseAuthToken", token);
            PlayerPrefs.SetString("UserEmail", email);
            PlayerPrefs.SetString("UserId", userId);
            PlayerPrefs.SetString("UserData", userData);
            PlayerPrefs.Save();
        }

        private void ClearAuthData()
        {
            PlayerPrefs.DeleteKey("FirebaseAuthToken");
            PlayerPrefs.DeleteKey("UserEmail");
            PlayerPrefs.DeleteKey("UserId");
            PlayerPrefs.DeleteKey("UserData");
            PlayerPrefs.Save();
        }

        private void LoadMainScene()
        {
            SceneManager.LoadScene(mainSceneName);
        }

        private void OpenRegistrationWebsite()
        {
            UpdateStatusText("Opening registration page...", Color.yellow);
            Application.OpenURL(registrationWebUrl);
            StartCoroutine(ResetStatusTextAfterDelay(2f));
        }

        private IEnumerator ResetStatusTextAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            UpdateStatusText("Register on website, then return to login", Color.white);
        }

        private void SetLoadingState(bool loading)
        {
            isLoading = loading;

            if (loginButton != null)
                loginButton.interactable = !loading;

            if (registerWebButton != null)
                registerWebButton.interactable = !loading;

            if (emailInput != null)
                emailInput.interactable = !loading;

            if (passwordInput != null)
                passwordInput.interactable = !loading;

            if (loadingIndicator != null)
                loadingIndicator.SetActive(loading);
        }

        private void UpdateStatusText(string message, Color color)
        {
            if (statusText != null)
            {
                statusText.text = message;
                statusText.color = color;
            }

            OnAuthStatusChanged?.Invoke(message);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public void Logout()
        {
            ClearAuthData();
            OnAuthenticationChanged?.Invoke(false);
            UpdateStatusText("Logged out successfully", Color.green);
            SceneManager.LoadScene("LandingPageScene");
        }

        public string GetCurrentUserEmail()
        {
            return PlayerPrefs.GetString("UserEmail", "");
        }

        public bool IsUserAuthenticated()
        {
            string token = PlayerPrefs.GetString("FirebaseAuthToken", "");
            return !string.IsNullOrEmpty(token);
        }

        public string GetAuthToken()
        {
            return PlayerPrefs.GetString("FirebaseAuthToken", "");
        }

        [System.Serializable]
        private class FirebaseErrorResponse
        {
            public FirebaseError error;
        }

        [System.Serializable]
        private class FirebaseError
        {
            public int code;
            public string message;
        }
    }
}