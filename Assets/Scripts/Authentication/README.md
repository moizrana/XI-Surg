# Meducator Authentication System

## 🩺 Overview

This authentication system provides a beautiful landing page for the Meducator Unity application where doctors can sign in using their website credentials and new users can register through the website.

## 🚀 Quick Setup

### Option 1: Automatic Setup (Recommended)

1. **Import the MeducatorAuthSetup script** - Already included in `Assets/Scripts/Authentication/`
2. **In your SampleScene**: 
   - Create an empty GameObject
   - Add the `MeducatorAuthSetup` component to it
   - The authentication UI will be created automatically when the scene loads

### Option 2  Manual Setup

1. **Add the AuthSetup Prefab** to your scene:
   - Drag `Assets/Prefabs/MeducatorAuthSetup.prefab` into your SampleScene
   - The authentication system will automatically initialize

## 🔧 Configuration

### Firebase Settings

The system is configured with your Firebase settings:
- **Database URL**: `https://meducator-c188d-default-rtdb.firebaseio.com/`
- **Registration URL**: `https://meducator.vercel.app/auth/login`

### Color Theme

The system uses a medical theme with:
- **Medical Blue**: For secondary buttons and accents
- **Medical Green**: For primary login button  
- **Dark Medical Blue**: For background panels
- **Light Blue**: For input field backgrounds

## 📱 Features

### Landing Page UI

✅ **Beautiful Medical Theme UI**
- Dark medical blue background
- Professional typography with emoji icons
- Responsive layout that scales with screen size

✅ **Authentication Form**
- Email input field with validation
- Secure password input field
- "Sign In to Meducator" primary button
- "New User? Register Here" secondary button

✅ **User Feedback**
- Status text for login progress
- Loading indicator during authentication
- Success/error messages

✅ **Registration Integration**
- "New User? Register Here" button opens: https://meducator.vercel.app/auth/login
- Users can register on the website and return to login

### Authentication Flow

🔄 **Automatic Authentication Check**
- Checks for saved authentication tokens on startup
- Automatically transitions to main scene if user is already logged in

🔐 **Login Process**
- Email and password validation
- Firebase authentication simulation (ready for real Firebase integration)
- Secure token storage using Unity PlayerPrefs

🎬 **Smooth Transitions**
- Fade-in animation when landing page loads
- Fade-out animation when transitioning to main scene
- Professional loading states

## 🛠 Scripts Overview

### FirebaseAuthManager.cs
- Handles all authentication logic
- Manages login/logout functionality
- Integrates with Firebase (currently using mock implementation)
- Provides events for other systems to subscribe to

### MeducatorAuthSetup.cs
- Automatically creates the entire authentication UI
- Handles UI element creation and styling
- Connects all components together
- Provides context menu options for manual setup/cleanup

### LandingPageManager.cs
- Manages UI interactions and animations
- Handles fade effects and transitions
- Manages keyboard input (Enter key navigation)

## 🔌 Integration with Main Scene

When a user successfully logs in:

1. **Authentication UI Fades Out** - Smooth transition effect
2. **Main Scene Reveals** - Original SampleScenario content becomes accessible
3. **Authentication Elements Removed** - Clean transition to the main application
4. **User Session Maintained** - Authentication state persists across sessions

## 🎨 Customization

### Changing Colors
Edit the `MeducatorAutoSetup.cs` script:
```csharp
[Header("Medical Theme Colors")]
public Color medicalBlue = new Color(0.2f, 0.4f, 0.8f, 1f);
public Color medicalGreen = new Color(0.3f, 0.7f, 0.4f, 1f);
public Color medicalBackground = new Color(0.05f, 0.1f, 0.2f, 0.95f);
```

### Modifying URLs
The registration URL can be changed in the `MeducatorAutoSetup.cs` script or in the inspector.

### Adding Real Firebase Integration

To integrate with real Firebase:

1. **Install Firebase SDK** for Unity
2. **Update FirebaseAuthManager.cs**:
   - Replace mock authentication methods
   - Add real Firebase API calls
   - Configure Firebase project settings

3. **Add Firebase Configuration**:
   ```csharp
   // In FirebaseAuthManager.cs
   private Firebase.Auth.FirebaseAuth auth;
   private Firebase.Auth.FirebaseUser user;
   ```

## 🧪 Testing

### Test Authentication
1. **Run the SampleScene**
2. **Try logging in** with any email/password (currently accepts any credentials for testing)
3. **Test registration button** - Should open the website
4. **Verify smooth transitions** to the main scene

### Test New User Flow
1. **Click "New User? Register Here"**
2. **Register on the website**
3. **Return to Unity app**
4. **Login with new credentials**

## 📝 Development Notes

- **Mock Authentication**: Currently uses placeholder authentication - replace with real Firebase SDK
- **Token Storage**: Uses Unity PlayerPrefs for session management
- **UI Creation**: Fully automated - no manual UI setup required
- **Responsive Design**: Scales with screen size automatically
- **Medical Theme**: Professional colors and fonts suitable for medical applications

## 🚨 Important Notes

1. **Firebase Integration**: Currently using mock authentication - integrate real Firebase SDK for production
2. **Security**: Replace mock token validation with proper Firebase token verification
3. **Testing**: Use any email/password combination for testing (will be updated when Firebase is integrated)
4. **Web Integration**: Users must register on the website first before logging into the Unity app

---

**Ready to use!** The authentication system will automatically create a beautiful landing page when you run your SampleScene. 🩺✨
