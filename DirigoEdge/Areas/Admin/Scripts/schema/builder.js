Schema.Builder = function (el, opts) {

    if (!opts && typeof el === 'object') {
        opts = el;
        el = null;
    }

    this.$el = el || $('.editSchema');

    this.defaults = {
        $schema: this.$el.find('.schema'),
        $fieldSelector: this.$el.find('.field-selector'),
    };

    this.settings = $.extend({}, this.defaults, opts);

};

Schema.Builder.prototype.init = function () {

    this.attachSelectorEvents();
    this.attachSaveEvents();

    $.when(EDGE.ajaxGet({}, '/areas/admin/scripts/schema/fake.json')).then(function (data) {
        this.remoteData = data;
        this.populateEditorFields();
        this.makeSortable();
        this.attachFieldEvents();
    }.bind(this));

};

Schema.Builder.prototype.populateEditorFields = function () {

    var _this = this;

    $.each(sortFields(this.remoteData.SchemaField), function () {

        _this.settings.$schema.find('.schema-fields').append(_this.renderEditorField(this.FieldType, _this.extendField(this)));

    });

    function sortFields(array) {

        array.forEach(function (item) {
            var keys = _.keys(item);
            keys.forEach(function (key) {
                if (_.isArray(item[key])) {
                    item[key] = sortFields(item[key]);
                }
            });
        });

        return _.sortBy(array, 'SortOrder');
    };

};

Schema.Builder.prototype.extendField = function (context) {

    context.extended = {
        metadata: Schema.definitions[context.FieldType].metadata,
        validation: Schema.definitions[context.FieldType].validation
    };

    return context;

};

Schema.Builder.prototype.makeSortable = function () {

    var _this = this;

    $('ul', this.settings.$schema).sortable({
        connectWith: $('ul', this.settings.$schema),
        placeholder: 'ui-state-highlight'
    });

    $('ul', this.settings.$fieldSelector).sortable({
        connectWith: $('ul', this.settings.$schema),
        placeholder: 'ui-state-highlight',
        start: function (event, ui) {
            ui.item.clone().removeAttr('style').addClass('clone').insertAfter(ui.item);
        },
        beforeStop: function (event, ui) {
            $(this).sortable('option', 'selfDrop', $(ui.placeholder).parent()[0] === this);
        },
        stop: function (event, ui) {
            var $sortable = $(this);

            if ($sortable.sortable('option', 'selfDrop')) {
                $('li.clone', this).remove();
                $sortable.sortable('cancel');
            } else {
                $('li.clone', this).removeClass('clone');
                _this.replaceSelectorWithField(ui);
            }
        }
    }).disableSelection();

};

Schema.Builder.prototype.attachSelectorEvents = function () {

    var _this = this;

    this.settings.$fieldSelector.find('li').on('click', function () {
        var type = $(this).data('type');
        _this.appendNewField(type);
    });

};

Schema.Builder.prototype.attachSaveEvents = function () {

    this.$el.find('#SaveSchemaButton').on('click', function () {

        var schema = this.remoteData;

        schema.SchemaField = this.serializeSchema();

        console.log(schema);

        /* todo: POST schema to server */

    }.bind(this));

};

Schema.Builder.prototype.attachFieldEvents = function () {

    var _this = this;

    this.settings.$schema.find('li').on('click', function (e) {
        var $this = $(this);

        e.stopPropagation();

        _this.settings.$schema.find('li.is-selected').removeClass('is-selected');

        $this.addClass('is-selected');
    });

};

Schema.Builder.prototype.renderEditorField = function (type, context) {

    if (!(type in Schema.templates.builderFields)) return null;

    context = context || Schema.definitions[type].defaultContext;

    return Schema.templates.builderFields[type](context);

};

Schema.Builder.prototype.replaceSelectorWithField = function (ui) {

    var type = ui.item.data('type');

    ui.item.replaceWith(this.renderEditorField(type));

};

Schema.Builder.prototype.appendNewField = function (type, context) {

    this.settings.$schema.find('ul').append(this.renderEditorField(type, context));

};

Schema.Builder.prototype.serializeSchema = function () {

    var fields = [];

    this.settings.$schema.children('li').each(function () {

        var type = $(this).data('type');

        fields.push(Schema.definitions[type].serialize($(this)));

    });

    return fields;

};