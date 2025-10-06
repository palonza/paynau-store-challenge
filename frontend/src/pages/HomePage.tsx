import React from "react";
import { useNavigate } from "react-router-dom";
import {
  Card,
  CardActionArea,
  CardContent,
  Typography,
  Grid,
  Box,
} from "@mui/material";
import Inventory2Icon from "@mui/icons-material/Inventory2";
import ReceiptLongIcon from "@mui/icons-material/ReceiptLong";

export default function HomePage() {
  const navigate = useNavigate();

  const sections = [
    {
      title: "Productos",
      description: "Gestioná el catálogo de productos disponibles.",
      icon: <Inventory2Icon sx={{ fontSize: 48 }} color="primary" />,
      path: "/productos",
    },
    {
      title: "Órdenes",
      description: "Creá y administrá las órdenes de compra.",
      icon: <ReceiptLongIcon sx={{ fontSize: 48 }} color="primary" />,
      path: "/ordenes",
    },
  ];

  return (
    <Box
      sx={{
    //         width: "100vw",
    // height: "100vh",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",p: 4,
    backgroundColor: "background.default",
        // minHeight: "100vh",
        // display: "flex",
        // alignItems: "center",
        // justifyContent: "center",
        // p: 4,
        // backgroundColor: "background.default",
      }}
    >
      <Grid container spacing={4} maxWidth="md" justifyContent="center">
        {sections.map((section) => (
          <Grid item xs={12} sm={6} key={section.title}>
            <Card
              elevation={3}
              sx={{
                borderRadius: 3,
                transition: "all 0.3s ease",
                "&:hover": {
                  transform: "translateY(-4px)",
                  boxShadow: 6,
                },
              }}
            >
              <CardActionArea onClick={() => navigate(section.path)}>
                <CardContent
                  sx={{
                    display: "flex",
                    flexDirection: "column",
                    alignItems: "center",
                    textAlign: "center",
                    p: 4,
                    gap: 2,
                  }}
                >
                  {section.icon}
                  <Typography variant="h5" fontWeight="bold">
                    {section.title}
                  </Typography>
                  <Typography
                    variant="body2"
                    color="text.secondary"
                    sx={{ mt: 1 }}
                  >
                    {section.description}
                  </Typography>
                </CardContent>
              </CardActionArea>
            </Card>
          </Grid>
        ))}
      </Grid>
    </Box>
  );
}
