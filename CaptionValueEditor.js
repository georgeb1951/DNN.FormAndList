/*
 * BCC Forms - "List of values" editor.
 *
 * Turns a text box holding "caption|value;caption|value;value" into a pencil
 * link that opens a caption/value grid in a DNN popup (jQuery UI dialog).
 *
 * DNN 10.3 ships jQuery UI 1.14, which ignores the old dialogClass option
 * (it is only honoured when $.uiBackCompat is set). Without DNN's dnnFormPopup
 * class the dialog had no background and sat underneath DNN's modal overlay,
 * so the dialog classes are set through the "classes" option instead.
 * The dialog is attached to <body>, outside the module, so it also gets the
 * bccForms class: that brings it into module.css's scope.
 */
(function ($) {
    'use strict';

    var defaults = {
        editLinkText: 'edit',
        editImageUrl: '~/images/edit.png',
        captionHeaderText: 'Caption',
        valueHeaderText: 'Value',
        addButtonText: 'add',
        addImageUrl: '~/images/add.gif',
        removeButtonText: 'remove',
        removeImageUrl: '~/images/delete.gif',
        saveButtonText: 'save',
        cancelButtonText: 'cancel',
        title: 'Edit',
        width: 460
    };

    var dialogClasses = 'dnnFormPopup dnnClear bccForms bccf-cve-dialog';

    $.fn.CaptionValueEditor = function (options) {
        var settings = $.extend({}, defaults, options);
        return this.each(function () {
            var input = $(this);
            if (!input.data('bccfCve')) {
                input.data('bccfCve', true);
                create(input, settings);
            }
        });
    };

    function iconButton(cssClass, text, imageUrl) {
        return $('<a href="#" role="button" class="bccf-cve-icon"></a>')
            .addClass(cssClass)
            .attr('title', text)
            .append($('<img>').attr({ src: imageUrl, alt: text }));
    }

    function create(input, s) {
        var tbody = $('<tbody></tbody>');
        var table = $('<table class="bccf-cve-table"></table>').append(
            $('<thead></thead>').append(
                $('<tr></tr>').append(
                    $('<th scope="col" class="bccf-cve-cmd"></th>').append($('<span class="bccf-sr"></span>').text(s.removeButtonText)),
                    $('<th scope="col"></th>').text(s.captionHeaderText),
                    $('<th scope="col"></th>').text(s.valueHeaderText))),
            tbody);

        var saveLink = $('<a href="#" role="button" class="dnnPrimaryAction"></a>').text(s.saveButtonText);
        var cancelLink = $('<a href="#" role="button" class="dnnSecondaryAction"></a>').text(s.cancelButtonText);

        var editor = $('<div class="bccf-cve"></div>').append(
            $('<div class="bccf-cve-scroll"></div>').append(table),
            $('<ul class="dnnActions dnnClear"></ul>').append(
                $('<li></li>').append(saveLink),
                $('<li></li>').append(cancelLink)));

        function textBox(value, label) {
            return $('<input type="text">').val(value).attr('aria-label', label);
        }

        // isLast: the trailing empty row, which shows "add" until it is used
        function addRow(caption, value, isLast) {
            var remove = iconButton('bccf-cve-remove', s.removeButtonText, s.removeImageUrl);
            var cmd = $('<td class="bccf-cve-cmd"></td>').append(remove);
            if (isLast) {
                remove.hide();
                cmd.append(iconButton('bccf-cve-add', s.addButtonText, s.addImageUrl));
            }
            var row = $('<tr></tr>').append(
                cmd,
                $('<td></td>').append(textBox(caption, s.captionHeaderText)),
                $('<td></td>').append(textBox(value, s.valueHeaderText)));
            tbody.append(row);
            return row;
        }

        function load() {
            tbody.empty();
            $.each(String(input.val() || '').split(';'), function (i, item) {
                var pipe = item.indexOf('|');
                if (pipe !== -1) {
                    addRow(item.substring(0, pipe), item.substring(pipe + 1), false);
                } else if ($.trim(item).length > 0) {
                    addRow('', item, false);
                }
            });
            addRow('', '', true);
        }

        function save() {
            var items = [];
            tbody.children('tr').each(function () {
                var boxes = $(this).find('input');
                var caption = $.trim(boxes.eq(0).val());
                var value = $.trim(boxes.eq(1).val());
                if (caption.length > 0 && value.length > 0) {
                    items.push(caption === value ? caption : caption + '|' + value);
                } else if (caption.length > 0 || value.length > 0) {
                    // a caption alone is also its value
                    items.push(caption || value);
                }
            });
            input.val(items.join(';')).trigger('change');
            editor.dialog('close');
        }

        editor.dialog({
            autoOpen: false,
            modal: true,
            resizable: true,
            title: s.title,
            width: s.width,
            height: 'auto',
            classes: { 'ui-dialog': dialogClasses },
            open: function () {
                tbody.find('input').first().trigger('focus');
            },
            close: function () {
                input.trigger('focus');
            }
        });

        editor.on('click', '.bccf-cve-remove', function (e) {
            e.preventDefault();
            $(this).closest('tr').remove();
        });

        editor.on('click', '.bccf-cve-add', function (e) {
            e.preventDefault();
            var row = $(this).closest('tr');
            var boxes = row.find('input');
            if ($.trim(boxes.eq(0).val()).length > 0 || $.trim(boxes.eq(1).val()).length > 0) {
                $(this).remove();
                row.find('.bccf-cve-remove').show();
                addRow('', '', true).find('input').first().trigger('focus');
            } else {
                boxes.first().trigger('focus');
            }
        });

        saveLink.on('click', function (e) {
            e.preventDefault();
            save();
        });

        cancelLink.on('click', function (e) {
            e.preventDefault();
            editor.dialog('close');
        });

        var openLink = iconButton('bccf-cve-open', s.editLinkText, s.editImageUrl);
        openLink.on('click', function (e) {
            e.preventDefault();
            load();
            // keep the popup inside narrow screens
            editor.dialog('option', 'width', Math.min(s.width, $(window).width() - 32));
            editor.dialog('open');
        });
        input.after(openLink);
    }
})(jQuery);
