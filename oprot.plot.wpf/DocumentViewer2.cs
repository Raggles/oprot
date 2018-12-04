using System.Windows.Controls;
using System.Windows.Xps;
using System.Printing;
using System.Windows.Documents;

namespace oprot.plot.wpf
{
    public class DocumentViewer2 : DocumentViewer
    {
        public PageMediaSizeName PageSize { get; set; }
        public PageOrientation Orientation { get; set; }

        protected override void OnPrintCommand()
        {
            // get a print dialog, defaulted to default printer and default printer's preferences.
            PrintDialog printDialog = new PrintDialog();
            printDialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
            printDialog.PrintTicket = printDialog.PrintQueue.DefaultPrintTicket;

            // get a reference to the FixedDocumentSequence for the viewer.
            FixedDocumentSequence docSeq = this.Document as FixedDocumentSequence;

            // set the default page orientation based on the desired output.
            printDialog.PrintTicket.PageOrientation = Orientation;
            printDialog.PrintTicket.PageMediaSize = new PageMediaSize(PageSize);

            if (printDialog.ShowDialog() == true)
            {
                // set the print ticket for the document sequence and write it to the printer.
                docSeq.PrintTicket = printDialog.PrintTicket;

                XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(printDialog.PrintQueue);
                writer.WriteAsync(docSeq, printDialog.PrintTicket);
            }
        }
    }
}