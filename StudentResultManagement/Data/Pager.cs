namespace StudentResultManagement.Data
{
    public class Pager
    {
        public int TotalItems { get; set; }// Total Data Items
        public int CurrentPage { get; set; }// Current Active Page
        public int PageSize { get; set; }// Numbers of items in a page



        public int TotalPages { get; set; }// Total number of pages
        public int StartPage { get; set; }// Starting page
        public int EndPage { get; set; }// Ending Page


        public Pager()
        {

        }

        public Pager(int totalItems, int page, int pageSize = 10)
        {
            int totalPages = (int)Math.Ceiling((double)totalItems / (double)pageSize);
            int currentPage = page;

            int startPage, endPage;

            startPage = currentPage - 5;
            endPage = currentPage + 4;


            if (startPage <= 0)
            {
                startPage = 1;
                endPage = (startPage + 9);

            }

            if (endPage > totalPages)
            {
                endPage = totalPages;
            }



            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;


        }
    }
}
