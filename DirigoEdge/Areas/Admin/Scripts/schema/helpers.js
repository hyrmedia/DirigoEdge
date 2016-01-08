Handlebars.registerHelper('list-item', function (context) {

    return Schema.templates.builderFields[context.FieldType](schemaBuilder.extendField(context));

});

Handlebars.registerHelper('render-metadata', function (context, metadata) {

    var templateName = _.camelCase(context);
    var metaObject = _.find(metadata, context);

    if (!metaObject || !(templateName in Schema.templates.metadata)) return '';

    return Schema.templates.metadata[templateName](metaObject);

});

Handlebars.registerHelper('render-validation', function (context, validation) {

    var templateName = _.camelCase(context);
    var validationObject = _.find(validation, context);

    if (!validationObject || !(templateName in Schema.templates.validation)) return '';

    return Schema.templates.metadata[templateName](validationObject);

});