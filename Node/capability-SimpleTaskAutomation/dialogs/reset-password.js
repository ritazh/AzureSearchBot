var builder = require('botbuilder');
var guid = require('guid');

const library = new builder.Library('resetPassword');

library.dialog('/', [
    function (session) {
        session.beginDialog('validators:phonenumber', {
            prompt: 'Please enter your phone number:',
            retryPrompt: 'The value entered is not phone number. Please try again using the following format (xyz) xyz-wxyz:',
            maxRetries: 2
        });
    },
    function (session, args) {
        if (args.resumed) {
            session.endDialog('You have tried to enter your phone number many times. Please try again later.');
            session.reset();
        } else {
            session.send('The phone you provided is: ' + args.response);
            session.dialogData.phoneNumber = args.response;
            builder.Prompts.time(session, 'Please enter your date of birth (MM/dd/yyyy):', {
                retryPrompt: 'The value you entered is not a valid date. Please try again:',
                maxRetries: 2
            });
        }
    },
    function (session, args) {
        if (args.resumed) {
            session.cancelDialog('You have tried to enter your date of birth many times. Please try again later.');
            session.reset();
        } else {        
            session.send('The date of birth you provided is: ' + args.response.entity);

            var newPassword = guid.create();
            session.endDialog('Thanks! Your new password is _' + newPassword.value + '_');
        }
    }
]).reloadAction('cancel', null, { matches: /^cancel/i });

module.exports = library;