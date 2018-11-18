-- phpMyAdmin SQL Dump
-- version 4.7.4
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 16-11-2018 a las 23:40:37
-- Versión del servidor: 10.1.29-MariaDB
-- Versión de PHP: 7.2.0

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `pruebawebtfg`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `carro`
--

CREATE TABLE `carro` (
  `IdCarro` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Volcado de datos para la tabla `carro`
--

INSERT INTO `carro` (`IdCarro`) VALUES
(1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `factura`
--

CREATE TABLE `factura` (
  `IdFactura` int(11) NOT NULL,
  `Fecha` varchar(255) NOT NULL,
  `Usuario` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Volcado de datos para la tabla `factura`
--

INSERT INTO `factura` (`IdFactura`, `Fecha`, `Usuario`) VALUES
(2, '2018-11-04 22:37:03', 'asm197'),
(3, '2018-11-04 22:46:35', 'asm197'),
(4, '2018-11-04 22:47:20', 'asm197');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `lineacarro`
--

CREATE TABLE `lineacarro` (
  `Cantidad` int(11) NOT NULL,
  `Producto` int(11) NOT NULL,
  `IdLinea` int(11) NOT NULL,
  `Carro` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Volcado de datos para la tabla `lineacarro`
--

INSERT INTO `lineacarro` (`Cantidad`, `Producto`, `IdLinea`, `Carro`) VALUES
(1, 499, 1, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `lineafactura`
--

CREATE TABLE `lineafactura` (
  `Cantidad` int(11) NOT NULL,
  `Producto` int(11) NOT NULL,
  `IdLinea` int(11) NOT NULL,
  `Factura` int(11) NOT NULL,
  `Precio` double NOT NULL,
  `NombreProducto` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Volcado de datos para la tabla `lineafactura`
--

INSERT INTO `lineafactura` (`Cantidad`, `Producto`, `IdLinea`, `Factura`, `Precio`, `NombreProducto`) VALUES
(1, 0, 2, 2, 1, 'Producto0'),
(1, 499, 3, 3, 50.900000000000006, 'Producto499'),
(3, 43, 4, 4, 5.3, 'Producto43'),
(1, 33, 5, 4, 4.300000000000001, 'Producto33');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `producto`
--

CREATE TABLE `producto` (
  `IdProducto` int(11) NOT NULL,
  `Nombre` varchar(255) NOT NULL,
  `Precio` double NOT NULL,
  `Descripcion` varchar(16000) NOT NULL,
  `ImagenPrincipal` varchar(255) NOT NULL,
  `Archivo` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuario`
--

CREATE TABLE `usuario` (
  `Username` varchar(50) NOT NULL,
  `Email` varchar(150) NOT NULL,
  `Contrasena` binary(64) NOT NULL,
  `Salt` binary(64) NOT NULL,
  `FechaRegistro` varchar(255) NOT NULL,
  `Nombre` varchar(150) NOT NULL,
  `Apellidos` varchar(150) NOT NULL,
  `Carro` int(11) NOT NULL,
  `Admin` tinyint(1) NOT NULL,
  `Saldo` double NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Volcado de datos para la tabla `usuario`
--

INSERT INTO `usuario` (`Username`, `Email`, `Contrasena`, `Salt`, `FechaRegistro`, `Nombre`, `Apellidos`, `Carro`, `Admin`, `Saldo`) VALUES
('asm197', 'asm197@alu.ua.es', 0x00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000, 0x5119fc262bf0c04445425d97f1b2eeb19e395c0459bd9b48e201f5e36b92de4da30e55deb95f7f4d516f62e36c88c2342e5dc71e67eb37345a101ed3ee422e76, '2018-10-24 12:56:43', 'Alejandro', 'Salvador Micó', 1, 1, 99926.89000000001);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `carro`
--
ALTER TABLE `carro`
  ADD PRIMARY KEY (`IdCarro`);

--
-- Indices de la tabla `factura`
--
ALTER TABLE `factura`
  ADD PRIMARY KEY (`IdFactura`);

--
-- Indices de la tabla `lineacarro`
--
ALTER TABLE `lineacarro`
  ADD PRIMARY KEY (`IdLinea`);

--
-- Indices de la tabla `lineafactura`
--
ALTER TABLE `lineafactura`
  ADD PRIMARY KEY (`IdLinea`);

--
-- Indices de la tabla `producto`
--
ALTER TABLE `producto`
  ADD PRIMARY KEY (`IdProducto`);

--
-- Indices de la tabla `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`Username`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
