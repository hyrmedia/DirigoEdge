Schema.Editor = function (el, opts) {

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

Schema.Editor.prototype.init = function () {

    this.populateEditorFields(this.makeSortable.bind(this));
    this.attachSelectorEvents();
    this.attachSaveEvents();

};

Schema.Editor.prototype.populateEditorFields = function (cb) {

    var _this = this;

    EDGE.ajaxGet({}, '/areas/admin/scripts/schema/fake.json', function (data) {

        _this.remoteData = data;

        $.each(data.SchemaField, function () {

            _this.settings.$schema.find('.schema-fields').append(_this.renderEditorField(this.FieldType, this));

        });

        if (cb && typeof cb === 'function') cb();

    });

};

Schema.Editor.prototype.makeSortable = function () {

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

Schema.Editor.prototype.attachSelectorEvents = function () {

    var _this = this;

    this.settings.$fieldSelector.find('li').on('click', function () {
        var type = $(this).data('type');
        _this.appendNewField(type);
    });

};

Schema.Editor.prototype.attachSaveEvents = function () {

    this.$el.find('#SaveSchemaButton').on('click', function () {

        var schema = this.remoteData;

        schema.SchemaField = this.serializeSchema();

        /* todo: POST schema to server */

    }.bind(this));

};

Schema.Editor.prototype.renderEditorField = function (type, context) {

    if (!(type in Schema.templates.fields)) return null;

    context = context || Schema.definitions[type].defaultContext;

    return Schema.templates.fields[type](context);

};

Schema.Editor.prototype.replaceSelectorWithField = function (ui) {

    var type = ui.item.data('type');

    ui.item.replaceWith(this.renderEditorField(type));

};

Schema.Editor.prototype.appendNewField = function (type, context) {

    this.settings.$schema.find('ul').append(this.renderEditorField(type, context));

};

Schema.Editor.prototype.serializeSchema = function () {

    var fields = [];

    this.settings.$schema.find('li').each(function () {

        var type = $(this).data('type');

        fields.push(Schema.definitions[type].serialize($(this)));

    });

    return fields;

};