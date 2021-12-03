namespace webapp.mvc.Services;

// Service that, reads the configuration for "page sizes" in the application from the
// application.json / application.Development.json / application.Release.json file
public class PageSizeService {
    public int PageSize { get; }
    public PageSizeService(int pageSize) {
        PageSize = pageSize;
    }
}