module.exports = function () {
    bot.dialog('/askQuestion', [
        function (session) {
            //Syntax for faceting results by 'Product'
            console.log('asking a question');
            var queryString = searchQueryStringBuilder('facet=Product_Name');
            console.log(queryString);
            performSearchQuery(queryString, function (err, result) {
                if (err) {
                    console.log("Error when faceting by product:" + err);
                } else if (result && result['@search.facets'] && result['@search.facets'].Product_Name) {
                    products = result['@search.facets'].Product_Name;
                    var productNames = [];
                    //Pushes the name of each product into an array
                    products.forEach(function (product, i) {
                        productNames.push(product['value'] + " (" + product.count + ")");
                    })    

                    console.log("Product_Names count: " + productNames.count);
                    //Prompts the user to select the product he/she is interested in
                    builder.Prompts.choice(session, "Which product is this question regarding?", productNames);
                } else {
                    session.endDialog("I couldn't find any products to show you");
                }
            })
        },
        function (session, results) {
            //Chooses just the product version - parsing out the count
            var productName = results.response.entity.split(' (')[0];
            session.userData.productName = productName;
            //Syntax for filtering results by 'product version'. Note the $ in front of filter (OData syntax)
            var queryString = searchQueryStringBuilder('facet=Product_Version&$filter=Product_Name eq ' + '\'' + session.userData.productName + '\'');
            console.log(queryString);
            performSearchQuery(queryString, function (err, result) {
                if (err) {
                    console.log("Error when faceting by product_version:" + err);
                } else if (result && result['@search.facets'] && result['@search.facets'].Product_Version) {
                    versions = result['@search.facets'].Product_Version;
                    var productVersions = [];
                    //Pushes the versions into an array
                    versions.forEach(function (version, i) {
                        productVersions.push(version['value'] + " (" + version.count + ")");
                    })    

                    console.log("productVersions count: " + productVersions.count);
                    //Prompts the user to select the product version he/she is interested in
                    builder.Prompts.choice(session, "Which version is this question regarding?", productVersions);
                } else {
                    session.endDialog("I couldn't find any product versions to show you");
                }
            })

        },
        function (session, results) {
            //Chooses just the product version - parsing out the count
            var productVersion = results.response.entity.split(' (')[0];
            session.userData.productVersion = productVersion;
            //Syntax for filtering results by 'product version'. Note the $ in front of filter (OData syntax)
            var queryString = searchQueryStringBuilder('facet=Product_OS&$filter=Product_Name eq ' + '\'' + session.userData.productName + '\' and Product_Version eq ' + '\'' + session.userData.productVersion + '\'' );
            console.log(queryString);
            performSearchQuery(queryString, function (err, result) {
                if (err) {
                    console.log("Error when faceting by product_os:" + err);
                } else if (result && result['@search.facets'] && result['@search.facets'].Product_OS) {
                    oss = result['@search.facets'].Product_OS;
                    var productOSs = [];
                    //Pushes the versions into an array
                    oss.forEach(function (os, i) {
                        productOSs.push(os['value'] + " (" + os.count + ")");
                    })    

                    console.log("productOSs count: " + productOSs.count);
                    //Prompts the user to select the product version he/she is interested in
                    builder.Prompts.choice(session, "Which OS is this question regarding?", productOSs);
                } else {
                    session.endDialog("I couldn't find any product OS to show you");
                }
            })

        },
        function (session, results) {
            //Chooses just the product version - parsing out the count
            var productOS = results.response.entity.split(' (')[0];
            session.userData.productOS = productOS;
            builder.Prompts.text(session, "What is your question?");

        },
        function (session, results) {
            //Chooses just the product name - parsing out the count
            var question = results.response;

            //Syntax for filtering results by 'product version'. Note the $ in front of filter (OData syntax)
            var queryString = searchQueryStringBuilder('search=\'' + question + '\'&$filter=Product_Name eq ' + '\'' + session.userData.productName + '\' and Product_Version eq ' + '\'' + session.userData.productVersion + '\' and Product_OS eq ' + '\'' + session.userData.productOS + '\'' );
            queryString = encodeURI(queryString);
            console.log(queryString);

            performSearchQuery(queryString, function (err, result) {
                console.log(result);

                if (err) {
                    console.log("Error getting a matching question: " + err);
                } else if (result && result['value'].length > 0) {
                    session.send('Total search results found: ' + result['value'].length);
                    //If we have results send them to the showResults dialog (acts like a decoupled view)
                    session.replaceDialog('/showResults', { result });
                    session.replaceDialog('/askQuestion');
                } else {
                    session.endDialog("I couldn't find any questions for this product :0");
                }
            })
        }
    ]);
}

