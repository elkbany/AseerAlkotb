# Payment API Testing Guide

This guide shows you how to test the Payment API endpoints that are running on `http://localhost:5234`.

## Current Status

✅ **Working**: Payment Methods endpoint  
❌ **Database Issue**: Payment table schema mismatch - missing Currency, PaymobOrderId, UserId columns  

## Setup Requirements

1. **Start the API**:
   ```bash
   dotnet run --project AseerAlkotb.API
   ```

2. **Database Schema Issue**: The Payment entity model includes properties (Currency, PaymobOrderId, UserId) that don't exist in the actual database table. This causes SQL errors when trying to query payments.

## Available Endpoints

### 1. ✅ **GET** `/api/payment/methods` - Get Payment Methods
Get all available payment methods. **This endpoint works correctly**.

```bash
curl -X GET "http://localhost:5234/api/payment/methods" \
  -H "accept: application/json"
```

**PowerShell**:
```powershell
Invoke-WebRequest -Uri "http://localhost:5234/api/payment/methods" -Method GET -ContentType "application/json"
```

Expected Response:
```json
{
  "succeeded": true,
  "data": [
    {"id": 1, "name": "Cash on Delivery", "code": "COD"},
    {"id": 2, "name": "Credit/Debit Card", "code": "Card"},
    {"id": 3, "name": "Mobile Wallet", "code": "Wallet"}
  ],
  "message": null
}
```

### 2. ❌ **POST** `/api/payment/initialize` - Initialize Payment
Initialize a payment for an order. **Currently fails due to database issues**.

```bash
curl -X POST "http://localhost:5234/api/payment/initialize" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "orderId": 1,
    "userId": 1,
    "paymentMethod": 1,
    "returnUrl": "http://localhost:5234/api/payment/callback",
    "notificationUrl": "http://localhost:5234/api/payment/notification"
  }'
```

**Payment Method Values:**
- `1` = Cash on Delivery (COD)
- `2` = Credit/Debit Card
- `3` = Mobile Wallet

**Current Issue**: Returns "Order not found" because there are no orders in the database.

### 3. ❌ **GET** `/api/payment/list` - Get Payments (Paginated)
Get a paginated list of payments with optional filters. **Currently fails due to database schema mismatch**.

```bash
# Basic request
curl -X GET "http://localhost:5234/api/payment/list?pageNumber=1&pageSize=10" \
  -H "accept: application/json"

# With filters
curl -X GET "http://localhost:5234/api/payment/list?pageNumber=1&pageSize=10&status=1&paymentMethod=1" \
  -H "accept: application/json"
```

**PowerShell**:
```powershell
Invoke-WebRequest -Uri "http://localhost:5234/api/payment/list?pageNumber=1&pageSize=10" -Method GET -ContentType "application/json"
```

**Current Error**: 
```
Invalid column name 'Currency'.
Invalid column name 'PaymobOrderId'.
Invalid column name 'UserId'.
```

**Status Values:**
- `1` = Pending
- `2` = Processing  
- `3` = Paid
- `4` = Failed
- `5` = Cancelled
- `6` = Refunded
- `7` = PartiallyRefunded

### 4. ❌ **GET** `/api/payment/{id}` - Get Payment by ID
Get details of a specific payment. **Currently fails due to database schema mismatch**.

```bash
curl -X GET "http://localhost:5234/api/payment/1" \
  -H "accept: application/json"
```

### 5. ❌ **PUT** `/api/payment/status` - Update Payment Status
Update the status of a payment (Admin only). **Currently fails due to database schema mismatch**.

```bash
curl -X PUT "http://localhost:5234/api/payment/status" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "paymentId": 1,
    "newStatus": 3,
    "notes": "Payment confirmed by admin"
  }'
```

## Database Issues and Solutions

### Current Problem
The Payment entity model has evolved and includes new properties that don't exist in the database:
- `Currency` (string)
- `PaymobOrderId` (long?)
- `UserId` (int)

### Temporary Solution for Testing
To fix the database schema, you need to manually add the missing columns:

```sql
-- Add missing columns to Payment table
ALTER TABLE [Payments] ADD [Currency] nvarchar(max) NOT NULL DEFAULT 'EGP';
ALTER TABLE [Payments] ADD [PaymobOrderId] bigint NULL;
ALTER TABLE [Payments] ADD [UserId] int NOT NULL DEFAULT 1;

-- Add foreign key constraint for UserId
ALTER TABLE [Payments] ADD CONSTRAINT FK_Payments_Users_UserId 
    FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE;

-- Add index for UserId
CREATE INDEX IX_Payments_UserId ON [Payments] ([UserId]);
```

### Order Data Requirement
To test payment initialization, you also need valid order data in the database:

```sql
-- Insert a test order (adjust values as needed)
INSERT INTO [Orders] ([OrderDate], [TotalAmount], [ShippingCost], [TaxAmount], [DiscountAmount], [FinalAmount], [PaymentMethod], [PaymentStatus], [Status], [TrackingNumber], [UserId], [City], [Governorate], [CreatedAt], [UpdatedAt])
VALUES (GETDATE(), 100.00, 10.00, 5.00, 0.00, 115.00, 1, 1, 1, 'TRK001', 1, 1, 1, GETDATE(), GETDATE());
```

## Testing with Swagger

The API has Swagger documentation available at:
`http://localhost:5234/swagger`

You can use Swagger UI to test the endpoints interactively.

## Working Test Example

**Test Payment Methods (This works)**:
```powershell
# Start the API first
dotnet run --project AseerAlkotb.API

# In another terminal, test the payment methods
Invoke-WebRequest -Uri "http://localhost:5234/api/payment/methods" -Method GET
```

Expected Output:
```
StatusCode        : 200
StatusDescription : OK
Content           : {"data":[{"id":1,"name":"Cash on Delivery","code":"COD"}...]}
```

## Notes

1. **Database Schema**: The main issue is database schema mismatch. The Payment entity has evolved but the database hasn't been updated properly.

2. **Order Dependencies**: Payment initialization requires valid orders in the database.

3. **User Dependencies**: Payment operations require valid user IDs.

4. **Logging**: Check the console output for detailed logs during API calls.

5. **Error Handling**: The API returns detailed error messages for debugging.

## Response Format

All API responses follow this format:
```json
{
  "succeeded": true/false,
  "data": "...",  // Response data
  "message": "...",  // Success/error message
  "errors": ["..."]  // List of errors (if any)
}
```

For paginated responses:
```json
{
  "succeeded": true,
  "data": [...],  // Array of items
  "totalCount": 100,
  "currentPage": 1,
  "totalPages": 10,
  "pageSize": 10,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

## Next Steps

1. **Fix Database Schema**: Add the missing columns to the Payment table
2. **Add Test Data**: Insert test orders and users for testing payment functionality
3. **Test All Endpoints**: Once database is fixed, test all payment endpoints
4. **Integration Testing**: Test the complete payment flow from initialization to completion