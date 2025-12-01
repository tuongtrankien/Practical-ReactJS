import Button from "@mui/material/Button";
import Card from "@mui/material/Card";
import CardActions from "@mui/material/CardActions";
import CardContent from "@mui/material/CardContent";
import CardMedia from "@mui/material/CardMedia";
import Typography from "@mui/material/Typography";
import { Product } from "../../models/Product";

interface ProductCardItemProps {
    product: Product;
}

const ProductCardItem = ({product} : ProductCardItemProps) => {
    return (
    <Card sx={{ maxWidth: 345 }}>
      <CardMedia
        component="img"
        alt={product.name}
        height="200"
        image={product.imageUrl}
        sx={{ objectFit: 'cover', width: '100%', height: '200px' }}
      />
      <CardContent>
        <Typography gutterBottom variant="h5" component="div">
          {product.name}
        </Typography>
        <Typography variant="body2" sx={{ color: 'text.secondary' }}>
          {product.description}
        </Typography>
      </CardContent>
      <CardActions>
        <Button size="small">Share</Button>
        <Button size="small">Learn More</Button>
      </CardActions>
    </Card>
  );
};

export default ProductCardItem;