#Paypal Bot

This bot works in much the same manner as AuthBot. It instructs the user to click a link to perform a payment on Paypal and points Paypal's "completion Url" back to a new Controller within the bot (`PaymentController`).

When the request is sent to `PaymentController`, the bot resumes the conversation using a stored ResumptionCookie for the user on the channel where the payment flow was initiated. It pulls relevant information from the Paypal response to know whether or not payment was successful at Paypal.