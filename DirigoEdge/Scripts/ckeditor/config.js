/**
 * @license Copyright (c) 2003-2012, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here.
	// For the complete reference:
	// http://docs.ckeditor.com/#!/api/CKEDITOR.config

	// The toolbar groups arrangement, optimized for two toolbar rows.
	config.toolbarGroups = [
		{ name: 'clipboard',   groups: [ 'clipboard', 'undo' ] },
		{ name: 'links' },
		{ name: 'insert' },
		{ name: 'tools' },
		{ name: 'document',	   groups: [ 'mode', 'document', 'doctools' ] },
		{ name: 'others' },
		'/',
		{ name: 'basicstyles', groups: [ 'basicstyles', 'cleanup' ] },
		{ name: 'paragraph',   groups: [ 'list', 'indent', 'blocks' ] },
		{ name: 'styles' },
		{ name: 'about' }
	];

	// Remove some buttons, provided by the standard plugins, which we don't
	// need to have in the standard toolbar.
	config.removeButtons = 'Underline,Subscript,Superscript,Image,Flash,Smiley,PageBreak,Iframe,ShowBlocks,Save,NewPage,Preview,Print,Templates,CreateDiv,Font,FontSize';
	config.removeDialogTabs = 'link:advanced';
	config.removePlugins = 'magicline';
	config.extraPlugins = 'codemirror,insertimage,removeresponsiveimage';
	config.disableNativeSpellChecker = false;
    config.allowedContent = true;
};
