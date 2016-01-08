var SchemaField = function (opts) {

    this.serialize = opts.serializer || this.defaultSerializer;
    this.defaultContext = opts.defaultContext;
    this.metadata = opts.metadata ? _.union(this.standardMetadata, opts.metadata) : this.standardMetadata;
    this.validation = opts.validation ? _.union(this.standardValidation, opts.validation) : this.standardValidation;

};

SchemaField.prototype.defaultSerializer = function () {

};

SchemaField.prototype.standardMetadata = [
    'Help Text',
    'Style'
];

SchemaField.prototype.standardValidation = [
];