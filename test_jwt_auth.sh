#!/bin/bash

# JWT Authentication Test Script
echo "🔐 Testing JWT Authentication Flow"
echo "=================================="

BASE_URL="http://localhost:5122/api/Account"

# Step 1: Login and get JWT tokens
echo ""
echo "📝 Step 1: Login to get JWT tokens"
echo "-----------------------------------"

LOGIN_RESPONSE=$(curl -s -X POST "$BASE_URL/login" \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@ascendid.local", "password": "AdminPassword123!"}')

echo "Login Response:"
echo "$LOGIN_RESPONSE" | python3 -m json.tool 2>/dev/null || echo "$LOGIN_RESPONSE"

# Extract tokens (requires jq for JSON parsing)
if command -v jq >/dev/null 2>&1; then
  ACCESS_TOKEN=$(echo "$LOGIN_RESPONSE" | jq -r '.accessToken')
  REFRESH_TOKEN=$(echo "$LOGIN_RESPONSE" | jq -r '.refreshToken')
  
  if [ "$ACCESS_TOKEN" != "null" ] && [ "$ACCESS_TOKEN" != "" ]; then
    echo ""
    echo "✅ Login successful!"
    echo "Access Token: ${ACCESS_TOKEN:0:50}..."
    echo "Refresh Token: ${REFRESH_TOKEN:0:50}..."
    
    # Step 2: Test protected endpoint
    echo ""
    echo "🔒 Step 2: Testing protected endpoint with JWT token"
    echo "------------------------------------------------"
    
    PROFILE_RESPONSE=$(curl -s -X GET "$BASE_URL/profile" \
      -H "Authorization: Bearer $ACCESS_TOKEN")
    
    echo "Profile Response:"
    echo "$PROFILE_RESPONSE" | python3 -m json.tool 2>/dev/null || echo "$PROFILE_RESPONSE"
    
    # Step 3: Test token refresh
    echo ""
    echo "🔄 Step 3: Testing token refresh"
    echo "-------------------------------"
    
    REFRESH_RESPONSE=$(curl -s -X POST "$BASE_URL/refresh-token" \
      -H "Content-Type: application/json" \
      -d "{\"refreshToken\": \"$REFRESH_TOKEN\"}")
    
    echo "Refresh Response:"
    echo "$REFRESH_RESPONSE" | python3 -m json.tool 2>/dev/null || echo "$REFRESH_RESPONSE"
    
    # Step 4: Test unauthorized access
    echo ""
    echo "❌ Step 4: Testing unauthorized access (should fail)"
    echo "------------------------------------------------"
    
    UNAUTHORIZED_RESPONSE=$(curl -s -w "HTTP Status: %{http_code}" -X GET "$BASE_URL/profile")
    
    echo "Unauthorized Response:"
    echo "$UNAUTHORIZED_RESPONSE"
    
    echo ""
    echo "🎉 JWT Authentication Test Complete!"
  else
    echo "❌ Login failed - no access token received"
  fi
else
  echo "⚠️  jq not installed - cannot parse JSON response"
  echo "Install jq with: brew install jq (on macOS)"
fi

echo ""
echo "💡 You can also test manually using the test_jwt.http file"