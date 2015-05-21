// Overwrite afterNewPageCreate method to redirect
// to newly created pagebuilder pages
console.log(content_class);

content_class.prototype.afterNewPageCreate = function (data) {

    console.log('Pagebuilder version');

    // Redirect to new page
    window.location = data.url;

};