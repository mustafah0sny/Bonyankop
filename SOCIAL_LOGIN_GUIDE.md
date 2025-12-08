# Social Login Integration Guide

## Overview
The Bonyankop API now supports social login with **Google**, **Facebook**, and **Apple**. This allows mobile users to authenticate using their social media accounts.

## API Endpoint

### POST `/api/user/social-login`

Authenticate or register a user using a social provider token.

**Request Body:**
```json
{
  "provider": "google",  // or "facebook", "apple"
  "idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjE...",  // ID token from provider
  "role": "CITIZEN"  // Optional: CITIZEN, ENGINEER, COMPANY, GOVERNMENT, ADMIN
}
```

**Success Response (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "user@example.com",
  "fullName": "John Doe",
  "role": "CITIZEN",
  "isVerified": true
}
```

**Error Responses:**
- `400 Bad Request`: Invalid provider or token
- `500 Internal Server Error`: Server error

---

## Mobile Implementation Guide

### 1. Google Sign-In

#### iOS (Swift)
```swift
import GoogleSignIn

func signInWithGoogle() {
    GIDSignIn.sharedInstance.signIn(withPresenting: self) { result, error in
        guard error == nil else { return }
        guard let user = result?.user,
              let idToken = user.idToken?.tokenString else { return }
        
        // Send to your API
        sendSocialLoginRequest(provider: "google", idToken: idToken)
    }
}
```

#### Android (Kotlin)
```kotlin
import com.google.android.gms.auth.api.signin.GoogleSignIn
import com.google.android.gms.auth.api.signin.GoogleSignInOptions

val gso = GoogleSignInOptions.Builder(GoogleSignInOptions.DEFAULT_SIGN_IN)
    .requestIdToken(getString(R.string.default_web_client_id))
    .requestEmail()
    .build()

val googleSignInClient = GoogleSignIn.getClient(this, gso)

// Launch sign-in intent
val signInIntent = googleSignInClient.signInIntent
startActivityForResult(signInIntent, RC_SIGN_IN)

// Handle result
override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
    super.onActivityResult(requestCode, resultCode, data)
    if (requestCode == RC_SIGN_IN) {
        val task = GoogleSignIn.getSignedInAccountFromIntent(data)
        val account = task.getResult(ApiException::class.java)
        val idToken = account?.idToken
        
        // Send to your API
        sendSocialLoginRequest("google", idToken)
    }
}
```

#### React Native
```javascript
import { GoogleSignin } from '@react-native-google-signin/google-signin';

GoogleSignin.configure({
  webClientId: 'YOUR_WEB_CLIENT_ID.apps.googleusercontent.com',
});

async function signInWithGoogle() {
  await GoogleSignin.hasPlayServices();
  const userInfo = await GoogleSignin.signIn();
  const idToken = userInfo.idToken;
  
  // Send to your API
  sendSocialLoginRequest('google', idToken);
}
```

---

### 2. Facebook Login

#### iOS (Swift)
```swift
import FBSDKLoginKit

let loginManager = LoginManager()
loginManager.logIn(permissions: ["public_profile", "email"], from: self) { result, error in
    guard error == nil else { return }
    guard let token = AccessToken.current?.tokenString else { return }
    
    // Facebook doesn't provide ID token directly, you need to exchange it
    // Or use the access token to get user info and create a custom token
    sendSocialLoginRequest(provider: "facebook", idToken: token)
}
```

#### Android (Kotlin)
```kotlin
import com.facebook.CallbackManager
import com.facebook.FacebookCallback
import com.facebook.login.LoginManager
import com.facebook.login.LoginResult

val callbackManager = CallbackManager.Factory.create()
LoginManager.getInstance().registerCallback(callbackManager, 
    object : FacebookCallback<LoginResult> {
        override fun onSuccess(result: LoginResult) {
            val token = result.accessToken.token
            sendSocialLoginRequest("facebook", token)
        }
        override fun onCancel() {}
        override fun onError(error: FacebookException) {}
    })

LoginManager.getInstance().logInWithReadPermissions(this, listOf("public_profile", "email"))
```

#### React Native
```javascript
import { LoginManager, AccessToken } from 'react-native-fbsdk-next';

async function signInWithFacebook() {
  const result = await LoginManager.logInWithPermissions(['public_profile', 'email']);
  if (result.isCancelled) return;
  
  const data = await AccessToken.getCurrentAccessToken();
  const idToken = data.accessToken;
  
  // Send to your API
  sendSocialLoginRequest('facebook', idToken);
}
```

---

### 3. Apple Sign-In

#### iOS (Swift)
```swift
import AuthenticationServices

func signInWithApple() {
    let request = ASAuthorizationAppleIDProvider().createRequest()
    request.requestedScopes = [.fullName, .email]
    
    let controller = ASAuthorizationController(authorizationRequests: [request])
    controller.delegate = self
    controller.presentationContextProvider = self
    controller.performRequests()
}

extension YourViewController: ASAuthorizationControllerDelegate {
    func authorizationController(controller: ASAuthorizationController, 
                                didCompleteWithAuthorization authorization: ASAuthorization) {
        if let credential = authorization.credential as? ASAuthorizationAppleIDCredential,
           let identityToken = credential.identityToken,
           let idTokenString = String(data: identityToken, encoding: .utf8) {
            
            // Send to your API
            sendSocialLoginRequest(provider: "apple", idToken: idTokenString)
        }
    }
}
```

#### Android (Kotlin)
```kotlin
// Apple Sign-In on Android requires additional setup
// Use a library like com.willowtreeapps.signinwithapplebutton
```

#### React Native
```javascript
import appleAuth from '@invertase/react-native-apple-authentication';

async function signInWithApple() {
  const appleAuthRequestResponse = await appleAuth.performRequest({
    requestedOperation: appleAuth.Operation.LOGIN,
    requestedScopes: [appleAuth.Scope.EMAIL, appleAuth.Scope.FULL_NAME],
  });
  
  const { identityToken } = appleAuthRequestResponse;
  
  // Send to your API
  sendSocialLoginRequest('apple', identityToken);
}
```

---

## API Call Example (JavaScript/TypeScript)

```typescript
async function sendSocialLoginRequest(provider: string, idToken: string, role?: string) {
  try {
    const response = await fetch('http://localhost:5000/api/user/social-login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        provider,
        idToken,
        role: role || 'CITIZEN'  // Default to CITIZEN if not specified
      }),
    });
    
    const data = await response.json();
    
    if (response.ok) {
      // Store the JWT token
      await AsyncStorage.setItem('authToken', data.token);
      await AsyncStorage.setItem('userId', data.userId);
      
      // Navigate to home screen
      navigation.navigate('Home');
    } else {
      console.error('Login failed:', data.message);
    }
  } catch (error) {
    console.error('Network error:', error);
  }
}
```

---

## Setup Instructions

### Backend Configuration (Already Done)
The API is configured to accept social login tokens. You need to:

1. **Get OAuth Credentials:**
   - Google: https://console.cloud.google.com/
   - Facebook: https://developers.facebook.com/
   - Apple: https://developer.apple.com/

2. **Update `appsettings.json`:**
   ```json
   {
     "Authentication": {
       "Google": {
         "ClientId": "YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com",
         "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
       },
       "Facebook": {
         "AppId": "YOUR_FACEBOOK_APP_ID",
         "AppSecret": "YOUR_FACEBOOK_APP_SECRET"
       }
     }
   }
   ```

### Mobile Configuration

#### Google
1. Create a project in Google Cloud Console
2. Enable Google Sign-In API
3. Create OAuth 2.0 credentials (Web client ID for iOS/Android)
4. Add SHA-1 fingerprint for Android
5. Add iOS bundle ID

#### Facebook
1. Create an app at Facebook Developers
2. Add Facebook Login product
3. Configure OAuth redirect URIs
4. Get App ID and App Secret

#### Apple
1. Register your app at Apple Developer
2. Enable Sign In with Apple capability
3. Create a Service ID
4. Configure redirect URIs

---

## Security Notes

1. **Never expose client secrets** in mobile apps
2. **Always validate tokens** on the backend
3. **Use HTTPS** in production
4. Store JWT tokens securely (Keychain on iOS, KeyStore on Android)
5. Implement token refresh mechanism for long-lived sessions

---

## Testing with Swagger

1. Start the API: `dotnet run`
2. Open Swagger: `http://localhost:5000`
3. Find the `/api/user/social-login` endpoint
4. Use a real ID token from Google/Facebook/Apple for testing

**Example Test Token (Google):**
```json
{
  "provider": "google",
  "idToken": "PASTE_REAL_GOOGLE_ID_TOKEN_HERE",
  "role": "CITIZEN"
}
```

---

## Flow Diagram

```
┌─────────────┐
│ Mobile App  │
└──────┬──────┘
       │
       │ 1. User taps "Sign in with Google"
       ▼
┌─────────────────┐
│ Google SDK      │
│ (Native/OAuth)  │
└──────┬──────────┘
       │
       │ 2. Returns ID Token
       ▼
┌─────────────────┐
│ Mobile App      │
│ (Your Code)     │
└──────┬──────────┘
       │
       │ 3. POST /api/user/social-login
       │    { provider, idToken, role }
       ▼
┌─────────────────┐
│ Bonyankop API   │
│ (Backend)       │
└──────┬──────────┘
       │
       │ 4. Validates token
       │ 5. Creates/finds user
       │ 6. Returns JWT token
       ▼
┌─────────────────┐
│ Mobile App      │
│ (Authenticated) │
└─────────────────┘
```

---

## Troubleshooting

### "Invalid ID token"
- Ensure the token is fresh (not expired)
- Verify you're using the ID token, not access token
- Check token is for the correct client ID

### "Email not found in token"
- Ensure email scope is requested
- Verify user granted email permission
- Check token claims contain email field

### "Account is deactivated"
- User previously deactivated their account
- Contact support to reactivate

---

## Support
For issues or questions, contact: support@bonyankop.com
