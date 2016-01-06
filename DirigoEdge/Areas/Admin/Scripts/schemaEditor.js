var SchemaEditor = function (el, opts) {

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

    this.templates = (function () {

        var obj = {};

        $('script[type="text/x-handlebars-template"]').each(function () {
            obj[$(this).attr('id').replace('field-template-', '')] = $(this).html();
        });

        return obj;

    }());

    /* todo: this should live somewhere else */
    this.defaultContexts = {
        text : {
            Label : 'Text'
        }
    };

};

SchemaEditor.prototype.init = function () {

    this.makeSortable();
    this.attachSelectorEvents();

};

SchemaEditor.prototype.makeSortable = function () {

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

SchemaEditor.prototype.attachSelectorEvents = function () {

    var _this = this;

    this.settings.$fieldSelector.find('li').on('click', function () {
        var type = $(this).data('type');
        _this.appendNewField(type);
    });

};

SchemaEditor.prototype.renderEditorField = function (type, context) {

    var template;

    if (!(type in this.templates)) return null;

    /* todo: pre-compile templates */
    template = Handlebars.compile(this.templates[type]);
    context = context || this.defaultContexts[type];

    return template(context);

};

SchemaEditor.prototype.replaceSelectorWithField = function (ui) {

    var type = ui.item.data('type');

    ui.item.replaceWith(this.renderEditorField(type));

};

SchemaEditor.prototype.appendNewField = function (type, context) {

    this.settings.$schema.find('ul').append(this.renderEditorField(type, context));

};

$(function () {
    var schemaEditor = new SchemaEditor();
    schemaEditor.init();
});