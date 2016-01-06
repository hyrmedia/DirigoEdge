Schema.definitions = {};

Schema.definitions.text = new SchemaField({
    serializer: function ($element) {
        return {
            SchemaFieldId: $element.data('id') ? parseInt($element.data('id')) : null,
            DisplayName: $element.find('.display-name').text(),
            FieldType: $element.data('type'),
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
            DisplayName: $element.find('.display-name').text(),
            FieldType: $element.data('type'),
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
            DisplayName: $element.find('.display-name').text(),
            FieldType: $element.data('type'),
            Metadata: {

            }
        }
    },
    defaultContext: {
        DisplayName: 'Textarea'
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
// List
// Module
// Number
// Group
// File
// Users
// Tags