import Box from "@mui/material/Box";
import ProductList from "../../components/product/ProductList";

const ProductPage = () => {
    return (
        <div>
            <h2>Product Page</h2>
            
            <Box sx={{ display: 'flex', flexDirection: 'column', justifyContent: 'flex-start', marginX: 4, minHeight: '100vh', padding: 2 }}>
                <ProductList />
            </Box>
        </div>
    );
}

export default ProductPage;