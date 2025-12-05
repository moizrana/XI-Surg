using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Meducator.Authentication
{
	public class FastApiAuthManager : MonoBehaviour
	{
		[Header("API Configuration")]
		public string apiBaseUrl = "https://meducator.onrender.com";
		public string registrationWebUrl = "https://meducator.vercel.app/auth/login";

		[Header("UI References")]
		public InputField emailInput;
		public InputField passwordInput;
		public Button loginButton;
		public Button registerButton;
		public Text statusText;
		public GameObject loadingIndicator;

		[Header("TMP UI (Optional)")]
		public TMPro.TMP_InputField emailTMPInput;
		public TMPro.TMP_InputField passwordTMPInput;

		[Header("Scene Settings")]
		public string mainSceneName = "SelectionScene";

		private bool isLoading = false;

		public static event Action<bool> OnAuthenticationChanged;
		public static event Action<string> OnAuthStatusChanged;

		[Serializable]
		private class LoginRequest
		{
			public string email;
			public string password;
		}

		[Serializable]
		private class TokenResponse
		{
			public string access_token;
			public string token_type;
			public UserResponse user;
		}

		[Serializable]
		public class UserResponse
		{
			public string id;
			public string email;
			public string first_name;
			public string last_name;
			public string role;
			public string department;
			public string specialization;
			public string created_at;
		}

		private void Start()
		{
			InitializeUI();
			CheckAuthenticationStatus();
		}

		private void InitializeUI()
		{
			// Attach listeners to any pre-wired UI (may be null at Start if wired later)
			if (loginButton != null)
			{
				loginButton.onClick.RemoveAllListeners();
				loginButton.onClick.AddListener(LoginUser);
			}
			if (registerButton != null)
			{
				registerButton.onClick.RemoveAllListeners();
				registerButton.onClick.AddListener(OpenRegistrationWebsite);
			}
			SetLoadingState(false);
			UpdateStatusText("Welcome to Meducator", Color.white);
		}

		public void RebindUI()
		{
			// Ensure current UI references are hooked up to handlers
			if (loginButton != null)
			{
				loginButton.onClick.RemoveAllListeners();
				loginButton.onClick.AddListener(LoginUser);
			}
			if (registerButton != null)
			{
				registerButton.onClick.RemoveAllListeners();
				registerButton.onClick.AddListener(OpenRegistrationWebsite);
			}
		}

		private void CheckAuthenticationStatus()
		{
			string token = PlayerPrefs.GetString("JwtToken", "");
			if (!string.IsNullOrEmpty(token)) StartCoroutine(ValidateTokenAndLoad(token));
		}

		private IEnumerator ValidateTokenAndLoad(string token)
		{
			// Do a non-blocking validation: keep inputs active while we check
			SetLoadingVisualOnly(true);
			UpdateStatusText("Validating session...", Color.yellow);

			using (UnityWebRequest req = UnityWebRequest.Get(BuildUrl("/auth/me")))
			{
				req.SetRequestHeader("Authorization", $"Bearer {token}");
				yield return req.SendWebRequest();

				if (req.result == UnityWebRequest.Result.Success)
				{
					OnAuthenticationChanged?.Invoke(true);
					UpdateStatusText("Session restored", Color.green);

					yield return new WaitForSeconds(0.8f);
					LoadMainScene();
				}
				else
				{
					PlayerPrefs.DeleteKey("JwtToken");
					UpdateStatusText("Session expired. Please login again.", Color.red);
					SetLoadingVisualOnly(false);
				}
			}
		}

		public void LoginUser()
		{
			if (isLoading) return;

			string email = emailInput != null ? emailInput.text : (emailTMPInput != null ? emailTMPInput.text : "");
			string password = passwordInput != null ? passwordInput.text : (passwordTMPInput != null ? passwordTMPInput.text : "");

			// Debug info to help diagnose empty input issues
			UnityEngine.Debug.Log($"FastApiAuthManager.LoginUser: emailInput={(emailInput!=null?emailInput.name:"null")}, emailTMP={(emailTMPInput!=null?emailTMPInput.name:"null")}, emailLen={(email!=null?email.Length:0)}");

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

			StartCoroutine(PerformLogin(email, password));
		}

		private IEnumerator PerformLogin(string email, string password)
		{
			SetLoadingState(true);
			UpdateStatusText("Signing in...", Color.yellow);

			LoginRequest body = new LoginRequest { email = email, password = password };
			string json = JsonUtility.ToJson(body);

			using (UnityWebRequest req = new UnityWebRequest(BuildUrl("/auth/login"), "POST"))
			{
				byte[] payload = Encoding.UTF8.GetBytes(json);
				req.uploadHandler = new UploadHandlerRaw(payload);
				req.downloadHandler = new DownloadHandlerBuffer();
				req.SetRequestHeader("Content-Type", "application/json");

				yield return req.SendWebRequest();

				if (req.result == UnityWebRequest.Result.Success)
				{
					TokenResponse token = null;
					bool parsedOk = false;
					try
					{
						token = JsonUtility.FromJson<TokenResponse>(req.downloadHandler.text);
						parsedOk = token != null;
					}
					catch (Exception e)
					{
						UnityEngine.Debug.LogError($"Login parse error: {e.Message} -> {req.downloadHandler.text}");
					}

					if (parsedOk && !string.IsNullOrEmpty(token.access_token))
					{
						SaveAuthData(token);
						OnAuthenticationChanged?.Invoke(true);
						OnAuthStatusChanged?.Invoke("Login successful");
						UpdateStatusText("Login successful!", Color.green);
						// Perform yield outside of try/catch per C# restrictions
						yield return new WaitForSeconds(1f);
						LoadMainScene();
					}
					else
					{
						UpdateStatusText("Login failed. Try again.", Color.red);
						SetLoadingState(false);
					}
				}
				else
				{
					string serverMessage = req.downloadHandler != null ? req.downloadHandler.text : "";
					UpdateStatusText(string.IsNullOrEmpty(serverMessage) ? "Invalid email or password" : serverMessage, Color.red);
					SetLoadingState(false);
				}
			}
		}

private void LoadMainScene()
		{
			// Transition to SelectionScene instead of SampleScene
			string selectionSceneName = "SelectionScene";
			if (!string.IsNullOrEmpty(selectionSceneName))
			{
				SceneManager.LoadScene(selectionSceneName);
			}
		}

		private void SaveAuthData(TokenResponse token)
		{
			PlayerPrefs.SetString("JwtToken", token.access_token);
			if (token.user != null)
			{
				PlayerPrefs.SetString("UserEmail", token.user.email ?? "");
				PlayerPrefs.SetString("UserId", token.user.id ?? "");
				PlayerPrefs.SetString("UserData", JsonUtility.ToJson(token.user));
			}
			PlayerPrefs.Save();
		}

		public void Logout()
		{
			StartCoroutine(PerformLogout());
		}

		private IEnumerator PerformLogout()
		{
			using (UnityWebRequest req = UnityWebRequest.PostWwwForm(BuildUrl("/auth/logout"), ""))
			{
				yield return req.SendWebRequest();
			}

			PlayerPrefs.DeleteKey("JwtToken");
			PlayerPrefs.DeleteKey("UserEmail");
			PlayerPrefs.DeleteKey("UserId");
			PlayerPrefs.DeleteKey("UserData");
			PlayerPrefs.Save();

			OnAuthenticationChanged?.Invoke(false);
			UpdateStatusText("Logged out", Color.green);
			SceneManager.LoadScene("LandingPageScene");
		}

		private string BuildUrl(string path)
		{
			if (string.IsNullOrEmpty(apiBaseUrl)) return path;
			if (apiBaseUrl.EndsWith("/")) apiBaseUrl = apiBaseUrl.TrimEnd('/');
			return apiBaseUrl + path;
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
			UpdateStatusText("Please register on the website, then login", Color.white);
		}

		private void SetLoadingState(bool loading)
		{
			isLoading = loading;
			if (loginButton != null) loginButton.interactable = !loading;
			if (registerButton != null) registerButton.interactable = !loading;
			if (emailInput != null) emailInput.interactable = !loading;
			if (passwordInput != null) passwordInput.interactable = !loading;
			if (emailTMPInput != null) emailTMPInput.interactable = !loading;
			if (passwordTMPInput != null) passwordTMPInput.interactable = !loading;
			if (loadingIndicator != null) loadingIndicator.SetActive(loading);
		}

		// Shows the loading indicator without disabling inputs/buttons
		private void SetLoadingVisualOnly(bool loading)
		{
			if (loadingIndicator != null) loadingIndicator.SetActive(loading);
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
	}
}



