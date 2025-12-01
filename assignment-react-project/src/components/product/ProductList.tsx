import Grid from "@mui/material/Grid";
import ProductCardItem from "./ProductCardItem";
import { useEffect, useState } from "react";
import { Product } from "../../models/Product";
import { api } from "../../api/axios";
import { PaginatedResult } from "../../models/PaginatedResult";
import Pagination from "@mui/material/Pagination";
import Box from "@mui/material/Box";

const ProductList = () => {
    const [loading, setLoading] = useState<boolean>(false);
    const [paginatedResult, setPaginatedResult] = useState<PaginatedResult<Product>>({
        items: [],
        totalCount: 0,
        currentPage: 1,
        pageSize: 3,
        totalPages: 0
    });

    useEffect(() => {
        setLoading(true);
        const fetchProducts = async () => {
            const response = await api.get<PaginatedResult<Product>>(
                `/product?pageNumber=${paginatedResult.currentPage}&pageSize=${paginatedResult.pageSize}`
            );
            setPaginatedResult(response.data);
        };
        fetchProducts();
        setLoading(false);
    }, [paginatedResult.currentPage, paginatedResult.pageSize]);

    return (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
            <Grid container spacing={2}>
                {paginatedResult.items.map((product) => (
                    <Grid size={4} key={product.id}>
                        <ProductCardItem product={product} />
                    </Grid>
                ))}
            </Grid>
            <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
                <Pagination
                    color="primary"
                    disabled={loading}
                    count={paginatedResult.totalPages}
                    page={paginatedResult.currentPage}
                    onChange={(event, page) =>
                        setPaginatedResult((prev) => ({ ...prev, currentPage: page }))
                    }
                />
            </Box>
        </Box>
    );
};

export default ProductList;
