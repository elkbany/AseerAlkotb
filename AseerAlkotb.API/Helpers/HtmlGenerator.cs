namespace AseerAlkotb.API.Helpers
{
    public static class HtmlGenerator
    {
        public static string GenerateSuccessHtml()
        {
            return @"
        <html>
            <head>
                <style>
                    body { font-family: Arial, sans-serif; background-color: #e8f5e9; color: #2e7d32; text-align: center; padding: 50px; }
                    h1 { font-size: 36px; }
                    p { font-size: 20px; }
                    .card { background: white; padding: 40px; border-radius: 10px; display: inline-block; box-shadow: 0 4px 8px rgba(0,0,0,0.2); }
                </style>
            </head>
            <body>
                <div class='card'>
                    <h1>Payment Successful!</h1>
                    <p>Thank you for your purchase. Your payment has been processed successfully.</p>
                </div>
            </body>
        </html>";
        }

        public static string GenerateFailureHtml()
        {
            return @"
        <html>
            <head>
                <style>
                    body { font-family: Arial, sans-serif; background-color: #ffebee; color: #c62828; text-align: center; padding: 50px; }
                    h1 { font-size: 36px; }
                    p { font-size: 20px; }
                    .card { background: white; padding: 40px; border-radius: 10px; display: inline-block; box-shadow: 0 4px 8px rgba(0,0,0,0.2); }
                </style>
            </head>
            <body>
                <div class='card'>
                    <h1>Payment Failed!</h1>
                    <p>Unfortunately, your payment could not be processed. Please try again.</p>
                </div>
            </body>
        </html>";
        }

        public static string GenerateSecurityHtml()
        {
            return @"
        <html>
            <head>
                <style>
                    body { font-family: Arial, sans-serif; background-color: #fff3e0; color: #ef6c00; text-align: center; padding: 50px; }
                    h1 { font-size: 36px; }
                    p { font-size: 20px; }
                    .card { background: white; padding: 40px; border-radius: 10px; display: inline-block; box-shadow: 0 4px 8px rgba(0,0,0,0.2); }
                </style>
            </head>
            <body>
                <div class='card'>
                    <h1>Security Check Failed!</h1>
                    <p>There was a problem verifying your payment. Please contact support.</p>
                </div>
            </body>
        </html>";
        }
    }


}
