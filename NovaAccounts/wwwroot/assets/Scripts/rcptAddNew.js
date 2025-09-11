 $(document).ready(function () {
            $('#items').empty();
            $('#items').load('/Receipts/_ReceiptItems?ReceiptID=' + encodeURI('@Model.ID'));
            //@*$('#TaxeItems').empty();
            //$('#TaxeItems').load('/Invoices/_SalesTaxe?InvoiceID=' + encodeURI(@Model.ID));*@
        });
        function AddNew() {
            $('.modal-dialog').empty();
            $('.modal-dialog').load('/Receipts/_AddItem?ReceiptID=' +encodeURI('@Model.ID')+ '&InvoiceID=' + encodeURI('@Model.InvoiceID'));
        };

        function PreviewInvoice() {
            $('.modal-dialog').empty();
            $('.modal-dialog').load('/Invoices/_PreviewInvoiceDetails?InvoiceID=' + encodeURI('@Model.InvoiceID'));
        };
        function RemoveItem(ItemID) {
            var jqxhr = $.get("/Receipts/RemoveReceiptDetailItem?ItemID=" + encodeURI(ItemID), function (data) {
                if (data.includes('success')) {
                    $('#items').empty();
                    $('#items').load('/Receipts/_ReceiptItems?ReceiptID=' + encodeURI('@Model.ID'));
                }
                else {
                    swal({
                        title: 'Error!',
                        type: 'error',
                        text: data
                    }
                    )
                }
            })
        };
        function Discard() {
             swal({
                 title: 'Are you sure?',
                 text: "You won't be able to revert this!",
                 type: 'error',
                 showCancelButton: true,
                 confirmButtonColor: '#29B4B6',
                 cancelButtonColor: '#E35B5A',
                 confirmButtonText: 'Yes, proceed!'
             }).then(function () {
                 var jqxhr = $.get("/Receipts/_DiscardReceipt?ReceiptID=" + encodeURI('@Model.ID'), function (data) {
                     if (data.includes('success')) {
                         swal(
                             'Success!',
                             data,
                             'success'
                         ).then(function () {
                             location.href = '/Receipts/Index';
                         })
                     }
                     else {
                         swal({
                             title: 'Error!',
                             type: 'error',
                             text: data
                         }
                         )
                     }
                 }).fail(function () {
                     swal(
                         'Error!',
                         'Your approval have not been saved. Please contact admin',
                         'error'
                     )
                 })
             }).catch(swal.noop);
        };