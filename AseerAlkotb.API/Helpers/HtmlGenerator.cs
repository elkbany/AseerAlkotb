﻿﻿namespace AseerAlkotb.API.Helpers
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
                    .redirect-btn { 
                        background-color: #4CAF50; 
                        border: none; 
                        color: white; 
                        padding: 15px 32px; 
                        text-align: center; 
                        text-decoration: none; 
                        display: inline-block; 
                        font-size: 16px; 
                        margin: 20px 2px; 
                        cursor: pointer; 
                        border-radius: 5px;
                    }
                    .redirect-btn:hover {
                        background-color: #45a049;
                    }
                    #countdown {
                        font-size: 18px;
                        margin: 20px 0;
                        font-weight: bold;
                    }
                </style>
                <script>
                    var countdownElement = document.getElementById('countdown');
                    var seconds = 10;
                    var redirectUrl = 'https://aseeralkotb.vercel.app/orders';
                    
                    function updateCountdown() {
                        if (countdownElement) {
                            countdownElement.textContent = 'Redirecting to website in ' + seconds + ' seconds...';
                        }
                    }
                    
                    function countdown() {
                        updateCountdown();
                        if (seconds <= 0) {
                            window.location.href = redirectUrl;
                        } else {
                            seconds--;
                            setTimeout(countdown, 1000);
                        }
                    }
                    
                    // Start countdown when page loads
                    if (document.readyState === 'loading') {
                        document.addEventListener('DOMContentLoaded', function() {
                            countdownElement = document.getElementById('countdown');
                            countdown();
                        });
                    } else {
                        countdownElement = document.getElementById('countdown');
                        countdown();
                    }
                </script>
            </head>
            <body>
                <div class='card'>
                    <h1>Payment Successful!</h1>
                    <p>Thank you for your purchase. Your payment has been processed successfully.</p>
                    <div id='countdown'>Redirecting to website in 10 seconds...</div>
                    <a href='https://aseeralkotb.vercel.app/orders' class='redirect-btn'>Go to Website Now</a>
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
                    .redirect-btn { 
                        background-color: #f44336; 
                        border: none; 
                        color: white; 
                        padding: 15px 32px; 
                        text-align: center; 
                        text-decoration: none; 
                        display: inline-block; 
                        font-size: 16px; 
                        margin: 20px 2px; 
                        cursor: pointer; 
                        border-radius: 5px;
                    }
                    .redirect-btn:hover {
                        background-color: #d32f2f;
                    }
                </style>
            </head>
            <body>
                <div class='card'>
                    <h1>Payment Failed!</h1>
                    <p>Unfortunately, your payment could not be processed. Please try again.</p>
                    <a href='https://aseeralkotb.vercel.app/orders' class='redirect-btn'>Back to Website</a>
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
                    .redirect-btn { 
                        background-color: #ff9800; 
                        border: none; 
                        color: white; 
                        padding: 15px 32px; 
                        text-align: center; 
                        text-decoration: none; 
                        display: inline-block; 
                        font-size: 16px; 
                        margin: 20px 2px; 
                        cursor: pointer; 
                        border-radius: 5px;
                    }
                    .redirect-btn:hover {
                        background-color: #f57c00;
                    }
                </style>
            </head>
            <body>
                <div class='card'>
                    <h1>Security Check Failed!</h1>
                    <p>There was a problem verifying your payment. Please contact support.</p>
                    <a href='https://aseeralkotb.vercel.app/orders' class='redirect-btn'>Back to Website</a>
                </div>
            </body>
        </html>";
        }
    }
}