export type PaginatedResult<T> = {
    items: T[];
    totalCount: number;
    currentPage: number;
    pageSize: number;
    totalPages: number;
};