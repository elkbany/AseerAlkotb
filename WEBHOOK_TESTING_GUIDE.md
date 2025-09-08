# 🧪 AseerAlkotb Webhook Testing Guide - UPDATED WITH FIXES

## 🎯 **Quick Setup Reference**

### **ngrok URLs (HTTP API → HTTPS tunnel)**
- **Public API**: `https://a8261666c995.ngrok-free.app`
- **Webhook URL**: `https://a8261666c995.ngrok-free.app/api/payment/webhook`
- **Callback URL**: `https://a8261666c995.ngrok-free.app/api/payment/callback`
- **Web Interface**: `http://127.0.0.1:4040`
- **Local API**: `http://localhost:5234` (HTTP)

## ✅ **FIXES APPLIED - UPDATED**
- **Empty Webhook Body**: ✅ Now properly handled with clear error messages
- **ngrok ERR_NGROK_3004**: ✅ Fixed HTML responses with proper Content-Type headers and Content-Length
- **Better Error Messages**: ✅ More descriptive validation and error handling
- **Enhanced Debugging**: ✅ Added comprehensive request/response logging
- **HTTP Header Issues**: ✅ Fixed duplicate header warnings, proper indexer usage
- **Content-Length Headers**: ✅ Explicit content length calculation for ngrok compatibility

---

## 🔧 **Testing Scenarios & Cases**

### **Scenario 1: Basic Endpoint Connectivity**

#### **Test 1.1: API Health Check**
```bash
# Test basic API connectivity
curl -X GET "https://32c3934ef794.ngrok-free.app/swagger/index.html"
```

**Expected**: Swagger UI should load successfully

#### **Test 1.2: Payment Endpoint Availability**
```bash
# Test webhook endpoint (should return 400 for empty body)
curl -X POST "https://32c3934ef794.ngrok-free.app/api/payment/webhook" \
  -H "Content-Type: application/json" \
  -d "{}"
```

**Expected**: 400 Bad Request with error message about invalid webhook structure

---

### **Scenario 2: Webhook Data Processing**

#### **Test 2.1: Valid Webhook Structure**
```bash
curl -X POST "https://32c3934ef794.ngrok-free.app/api/payment/webhook" \
  -H "Content-Type: application/json" \
  -d '{
    "type": "TRANSACTION",
    "obj": {
      "id": 123456789,
      "success": true,
      "amount_cents": 50000,
      "pending": false,
      "created_at": "2024-01-15T10:30:00Z",
      "currency": "EGP",
      "error_occured": false,
      "order": {
        "id": 987654321,
        "merchant_order_id": "TXN_1_1_1705315800_1234",
        "amount_cents": 50000,
        "currency": "EGP",
        "payment_status": "PAID"
      },
      "source_data": {
        "type": "card",
        "sub_type": "visa",
        "pan": "****1234"
      }
    }
  }'
```

**Expected**: 
- 200 OK with `{"status": "success"}` if payment found
- 400 Bad Request if payment not found

#### **Test 2.2: Failed Payment Webhook**
```bash
curl -X POST "https://32c3934ef794.ngrok-free.app/api/payment/webhook" \
  -H "Content-Type: application/json" \
  -d '{
    "type": "TRANSACTION",
    "obj": {
      "id": 123456790,
      "success": false,
      "amount_cents": 50000,
      "pending": false,
      "created_at": "2024-01-15T10:30:00Z",
      "currency": "EGP",
      "error_occured": true,
      "order": {
        "id": 987654322,
        "merchant_order_id": "TXN_1_1_1705315800_5678",
        "amount_cents": 50000,
        "currency": "EGP",
        "payment_status": "FAILED"
      }
    }
  }'
```

**Expected**: Payment status should be updated to Failed

---

### **Scenario 3: Callback Testing (User Redirects) - UPDATED FOR ERR_NGROK_3004 FIX**

#### **Test 3.1: Test Your Exact Callback URL (ERR_NGROK_3004 Fix)**
```
Open in browser (this is your exact callback that was failing):
https://7c7e2a2bf1f4.ngrok-free.app/api/payment/callback?id=340025798&pending=false&amount_cents=46040&success=true&is_auth=false&is_capture=false&is_standalone_payment=true&is_voided=false&is_refunded=false&is_3d_secure=true&integration_id=5235556&profile_id=1068418&has_parent_transaction=false&order=381909604&created_at=2025-09-06T18%3A25%3A46.977136&currency=EGP&merchant_commission=0&accept_fees=0&discount_details=%5B%5D&is_void=false&is_refund=false&error_occured=false&refunded_amount_cents=0&captured_amount=0&updated_at=2025-09-06T18%3A26%3A10.843712&is_settled=false&bill_balanced=false&is_bill=false&owner=2022100&merchant_order_id=141534313&data.message=Approved&source_data.type=card&source_data.pan=8769&source_data.sub_type=Visa&acq_response_code=00&txn_response_code=APPROVED&hmac=773e4b1ff3083647ecd44c92f513c6d3c05f7fbe03abfbbe59cb2b9a6fbb631bb0a2855bd9da4daeca6965b8a1b7d69ea7ded9d33c072774dc5b987fcff2f81c
```

**Expected**: Success HTML page displayed WITHOUT ERR_NGROK_3004 error

#### **Test 3.2: Successful Payment Callback (GET) - Simple**
```
Open in browser:
https://32c3934ef794.ngrok-free.app/api/payment/callback?merchant_order_id=TXN_1_1_1705315800_1234&success=true&amount_cents=50000
```

**Expected**: Success HTML page displayed

#### **Test 3.2: Failed Payment Callback (GET)**
```
Open in browser:
https://32c3934ef794.ngrok-free.app/api/payment/callback?merchant_order_id=TXN_1_1_1705315800_5678&success=false&amount_cents=50000
```

**Expected**: Failure HTML page displayed

#### **Test 3.3: POST Callback (Form Data)**
```bash
curl -X POST "https://32c3934ef794.ngrok-free.app/api/payment/callback" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "merchant_order_id=TXN_1_1_1705315800_1234&success=true&amount_cents=50000&currency=EGP"
```

**Expected**: Success HTML response

---

### **Scenario 4: Security Testing**

#### **Test 4.1: HMAC Validation (with valid HMAC)**
```bash
# Note: You'll need to calculate the HMAC SHA512 for this body
curl -X POST "https://32c3934ef794.ngrok-free.app/api/payment/webhook?hmac=your_calculated_hmac" \
  -H "Content-Type: application/json" \
  -d '{
    "type": "TRANSACTION",
    "obj": {
      "id": 123456789,
      "success": true,
      "amount_cents": 50000,
      "pending": false,
      "created_at": "2024-01-15T10:30:00Z",
      "currency": "EGP",
      "error_occured": false,
      "order": {
        "id": 987654321,
        "merchant_order_id": "TXN_1_1_1705315800_1234",
        "amount_cents": 50000,
        "currency": "EGP",
        "payment_status": "PAID"
      }
    }
  }'
```

#### **Test 4.2: Invalid HMAC**
```bash
curl -X POST "https://32c3934ef794.ngrok-free.app/api/payment/webhook?hmac=invalid_hmac" \
  -H "Content-Type: application/json" \
  -d '{"type": "TRANSACTION", "obj": {}}'
```

**Expected**: 401 Unauthorized

---

### **Scenario 5: Edge Cases & Error Handling**

#### **Test 5.1: Malformed JSON**
```bash
curl -X POST "https://32c3934ef794.ngrok-free.app/api/payment/webhook" \
  -H "Content-Type: application/json" \
  -d '{"type": "TRANSACTION", "obj": {'
```

**Expected**: 400 Bad Request with JSON parsing error

#### **Test 5.2: Missing Required Fields**
```bash
curl -X POST "https://32c3934ef794.ngrok-free.app/api/payment/webhook" \
  -H "Content-Type: application/json" \
  -d '{
    "type": "TRANSACTION",
    "obj": {
      "id": 123456789
    }
  }'
```

**Expected**: 400 Bad Request with validation error

#### **Test 5.3: Duplicate Webhook Processing**
```bash
# Send the same webhook twice
curl -X POST "https://32c3934ef794.ngrok-free.app/api/payment/webhook" \
  -H "Content-Type: application/json" \
  -d '{
    "type": "TRANSACTION",
    "obj": {
      "id": 123456789,
      "success": true,
      "amount_cents": 50000,
      "pending": false,
      "created_at": "2024-01-15T10:30:00Z",
      "currency": "EGP",
      "error_occured": false,
      "order": {
        "id": 987654321,
        "merchant_order_id": "TXN_1_1_1705315800_1234",
        "amount_cents": 50000,
        "currency": "EGP",
        "payment_status": "PAID"
      }
    }
  }'
```

**Expected**: First call processes, second call returns "Payment already processed"

---

## 📊 **Monitoring & Debugging**

### **1. ngrok Web Interface**
- Open: `http://127.0.0.1:4040`
- View all incoming requests in real-time
- Inspect request/response details
- Replay requests for debugging

### **2. Application Logs**
Monitor your console where the API is running for detailed logs:
```
info: AseerAlkotb.API.Controllers.PaymentController[0]
      Received Paymob webhook: {"type":"TRANSACTION","obj":{...}}
```

### **3. Database Verification**
Check payment status updates in your database:
```sql
SELECT Id, TransactionId, Status, PaymentDate, ProviderPayload 
FROM Payments 
ORDER BY PaymentDate DESC
```

---

## 🎯 **Production Checklist**

### **Before Going Live:**

1. **✅ Configure Real URLs in Paymob Dashboard**
   - Replace ngrok URLs with your production domain
   - Webhook: `https://yourdomain.com/api/payment/webhook`
   - Callback: `https://yourdomain.com/api/payment/callback`

2. **✅ Security Configuration**
   - Ensure HMAC secret is configured in appsettings.json
   - Enable HTTPS only in production
   - Configure proper CORS settings

3. **✅ Logging & Monitoring**
   - Set up proper logging levels for production
   - Configure application insights/monitoring
   - Set up alerts for payment processing failures

4. **✅ Database Backups**
   - Ensure payment data is properly backed up
   - Test disaster recovery procedures

5. **✅ Load Testing**
   - Test webhook handling under load
   - Verify payment processing performance

---

## 🚨 **Troubleshooting Common Issues**

### **Issue 1: Webhook Not Received**
- Check ngrok is running and URL is correct
- Verify Paymob Dashboard configuration
- Check firewall/network settings

### **Issue 2: Payment Not Found**
- Verify transaction ID format matches
- Check payment matching logic (amount + time)
- Review payment creation process

### **Issue 3: HMAC Validation Fails**
- Verify HMAC secret in configuration
- Check HMAC calculation algorithm
- Ensure body content is exactly as received

### **Issue 4: Database Connection Issues**
- Check connection string configuration
- Verify database is accessible
- Review Entity Framework migrations

---

## 📞 **Support Commands**

### **Restart Services**
```bash
# Restart API
Ctrl+C (in API terminal)
dotnet run --project AseerAlkotb.API

# Restart ngrok  
Ctrl+C (in ngrok terminal)
ngrok http 5234
```

### **Check Service Status**
```bash
# Check if API is running
netstat -an | findstr 5234

# Check ngrok status
curl http://127.0.0.1:4040/api/tunnels
```