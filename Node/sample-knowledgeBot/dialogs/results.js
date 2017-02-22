module.exports = function () {
    bot.dialog('/showResults', [
        function (session, args) {
            var msg = new builder.Message(session).attachmentLayout(builder.AttachmentLayout.carousel);
                args.result['value'].forEach(function (question, i) {
                    msg.addAttachment(
                        new builder.HeroCard(session)
                            .title(question.Name)
                            .subtitle("Product: " + question.Product + " | " + "Search Score: " + question['@search.score'])
                            .text(question.Description)
                            //.images([builder.CardImage.create(session, question.imageURL)])
                    );
                })
                session.endDialog(msg);
        }
    ])
}