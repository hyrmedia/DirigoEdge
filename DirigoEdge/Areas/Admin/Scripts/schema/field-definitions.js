Schema.definitions = {};

Schema.definitions.text = new SchemaField({
    serializer: function ($element) {
        return {
            SchemaFieldId: $element.data('id') ? parseInt($element.data('id')) : null,
            ParentSchemaFieldId: $element.parent().closest('.field-list').data('id'),
            DisplayName: $element.find('.display-name').first().text(),
            FieldType: $element.data('type'),
            SortOrder: null,
            Metadata: {

            }
        }
    },
    defaultContext: {
        DisplayName: 'Text'
    }
});

Schema.definitions.image = new SchemaField({
    serializer: function ($element) {
        return {
            SchemaFieldId: $element.data('id') ? parseInt($element.data('id')) : null,
            ParentSchemaFieldId: $element.parent().closest('.field-list').data('id'),
            DisplayName: $element.find('.display-name').first().text(),
            FieldType: $element.data('type'),
            SortOrder: null,
            Metadata: {

            }
        }
    },
    defaultContext: {
        DisplayName: 'Image'
    }
});

Schema.definitions.textarea = new SchemaField({
    serializer: function ($element) {
        return {
            SchemaFieldId: $element.data('id') ? parseInt($element.data('id')) : null,
            ParentSchemaFieldId: $element.parent().closest('.field-list').data('id'),
            DisplayName: $element.find('.display-name').first().text(),
            FieldType: $element.data('type'),
            SortOrder: null,
            Metadata: {

            }
        }
    },
    defaultContext: {
        DisplayName: 'Textarea'
    }
});

Schema.definitions.list = new SchemaField({
    serializer: function ($element) {
        return {
            SchemaFieldId: $element.data('id') ? parseInt($element.data('id')) : null,
            ParentSchemaFieldId: $element.parent().closest('.field-list').data('id'),
            DisplayName: $element.find('.display-name').first().text(),
            FieldType: $element.data('type'),
            SortOrder: null,
            Metadata: {

            }
        }
    },
    defaultContext: {
        DisplayName: 'List'
    }
});

/* todo: Finish field definitions */

// Checkbox
// Select
// Radio
// Date
// Time
// Date + Time
// WYSIWYG
// Module
// Number
// Group
// File
// Users
// Tags