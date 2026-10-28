# Using JWT Tokens in Swagger UI

## 🚀 Quick Start Guide

### Step 1: Start the Application
```bash
dotnet run --project src/AscendID_API.WebAPI/AscendID_API.WebAPI.csproj --urls "http://localhost:5122"
```

### Step 2: Open Swagger UI
Navigate to: `http://localhost:5122/swagger`

### Step 3: Login to Get JWT Token

1. **Find the login endpoint**: Look for `POST /api/Account/login`
2. **Click "Try it out"**
3. **Enter credentials**:
   ```json
   {
     "email": "admin@ascendid.local",
     "password": "AdminPassword123!"
   }
   ```
4. **Execute** and copy the `accessToken` from the response

### Step 4: Authorize in Swagger

1. **Click the "Authorize" button** (🔒 lock icon) at the top right of Swagger UI
2. **Enter your token** in the "Value" field: `Bearer YOUR_ACCESS_TOKEN_HERE`
   - ⚠️ **Important**: Make sure to include "Bearer " before your token
   - Example: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
3. **Click "Authorize"**
4. **Click "Close"**

### Step 5: Test Protected Endpoints

Now you can call any protected endpoint (those with a 🔒 lock icon):
- `GET /api/Account/profile` - Get user profile
- `PUT /api/Account/profile` - Update profile  
- `POST /api/Account/change-password` - Change password
- `POST /api/Account/logout` - Logout

### 🔄 Token Refresh

When your token expires (default: 60 minutes in Local environment):
1. Use `POST /api/Account/refresh-token` with your `refreshToken`
2. Get new tokens and repeat Step 4 with the new `accessToken`

### 💡 Pro Tips

- **Token Format**: Always prefix with "Bearer " (note the space)
- **Copy Carefully**: Ensure you copy the complete token without extra characters
- **Check Expiry**: Tokens expire after the configured time (see `appsettings.Local.json`)
- **Logout**: Use the logout endpoint to invalidate tokens server-side

### 🔍 Troubleshooting

- **401 Unauthorized**: Check if token is expired or malformed
- **403 Forbidden**: Token is valid but user lacks permissions  
- **Missing Bearer prefix**: Make sure to include "Bearer " before the token
- **Token not working**: Try logging in again to get a fresh token