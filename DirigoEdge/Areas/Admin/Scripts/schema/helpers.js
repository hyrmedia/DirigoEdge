Handlebars.registerHelper('list-item', function (context) {

    var ctx = Array.isArray(context) ? _.sortBy(context, 'SortOrder') : context;

    // todo: sortBy doesn't work maybe because context is never an array?

    console.log(ctx);

    return Schema.templates.fields[context.FieldType](ctx);

});