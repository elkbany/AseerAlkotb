@echo off
chcp 65001 >nul
echo === Testing Session Entity Caching ===
echo.

echo 🔸 Test 1: First request (should cache author)
echo Making first request...

curl -s -X POST "https://localhost:7207/api/Rag/ask" ^
  -H "accept: application/json" ^
  -H "Content-Type: application/json" ^
  -d "{\"question\": \"من هو نجيب محفوظ؟\", \"limit\": 5}" ^
  -D headers1.txt ^
  -o response1.json

if %errorlevel% neq 0 (
    echo ❌ ERROR: Could not connect to API
    echo Make sure the API is running on https://localhost:7207
    pause
    exit /b 1
)

echo ✅ First request completed

REM Extract session ID from headers
for /f "tokens=2 delims=: " %%a in ('findstr /i "X-Session-Id" headers1.txt') do set SESSION_ID=%%a
set SESSION_ID=%SESSION_ID: =%

echo ✅ Session ID extracted: %SESSION_ID%
echo.

echo 🔸 Test 2: Second request (should use cached author)
echo Making second request with session ID...

curl -s -X POST "https://localhost:7207/api/Rag/ask" ^
  -H "accept: application/json" ^
  -H "Content-Type: application/json" ^
  -H "X-Session-Id: %SESSION_ID%" ^
  -d "{\"question\": \"ما هي أشهر كتبه؟\", \"limit\": 5}" ^
  -o response2.json

if %errorlevel% neq 0 (
    echo ❌ ERROR: Second request failed
    pause
    exit /b 1
)

echo ✅ Second request completed
echo.

echo === Results ===
echo Session ID: %SESSION_ID%
echo.
echo First Response:
type response1.json | jq -r .data.answer 2>nul || echo "Install jq to see formatted output, or check response1.json manually"
echo.
echo Second Response:
type response2.json | jq -r .data.answer 2>nul || echo "Install jq to see formatted output, or check response2.json manually"

echo.
echo === Analysis ===
findstr /c:"برجاء تحديد" response2.json >nul
if %errorlevel% equ 0 (
    echo ❌ FAILED: System asked to specify author - caching not working
) else (
    echo ✅ SUCCESS: System likely used cached author context!
)

echo.
echo Check response1.json and response2.json for detailed output
echo.
pause

REM Cleanup
del headers1.txt response1.json response2.json 2>nul