-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 19-12-2025 a las 08:16:18
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `sistema_gestion_clinica`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `citas`
--

CREATE TABLE `citas` (
  `id` int(11) NOT NULL,
  `paciente_id` int(11) NOT NULL,
  `medico_id` int(11) NOT NULL,
  `fecha_hora` datetime NOT NULL,
  `motivo` varchar(255) DEFAULT NULL,
  `estado` varchar(20) NOT NULL DEFAULT 'Programada',
  `activo` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NULL DEFAULT NULL ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `citas`
--

INSERT INTO `citas` (`id`, `paciente_id`, `medico_id`, `fecha_hora`, `motivo`, `estado`, `activo`, `created_at`, `updated_at`) VALUES
(1, 1, 1, '2025-01-10 09:00:00', 'Dolor de cabeza y revisión general', 'Confirmada', 1, '2025-12-19 01:22:59', NULL),
(2, 2, 2, '2025-01-10 10:30:00', 'Control pediátrico (hijo del paciente)', 'Programada', 1, '2025-12-19 01:22:59', NULL),
(3, 3, 3, '2025-01-11 08:15:00', 'Chequeo cardiovascular', 'Programada', 1, '2025-12-19 01:22:59', NULL),
(4, 4, 4, '2025-12-21 00:52:00', 'Dermatitis', 'Programada', 1, '2025-12-19 05:53:05', NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `medicos`
--

CREATE TABLE `medicos` (
  `id` int(11) NOT NULL,
  `nombres` varchar(100) NOT NULL,
  `apellidos` varchar(100) NOT NULL,
  `especialidad` varchar(100) NOT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `email` varchar(120) DEFAULT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NULL DEFAULT NULL ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `medicos`
--

INSERT INTO `medicos` (`id`, `nombres`, `apellidos`, `especialidad`, `telefono`, `email`, `activo`, `created_at`, `updated_at`) VALUES
(1, 'Carlos', 'Mendoza Torres', 'Medicina General', '0990001112', 'cmendoza@clinica.com', 1, '2025-12-19 01:21:29', NULL),
(2, 'Valeria', 'Ramírez López', 'Pediatría', '0982223344', 'vramirez@clinica.com', 1, '2025-12-19 01:21:29', NULL),
(3, 'Jorge', 'Salazar Peña', 'Cardiología', '0975556677', 'jsalazar@clinica.com', 1, '2025-12-19 01:21:29', NULL),
(4, 'Patricia Chávez', 'Alexandra Panamito', 'Dermatología', '123456789', 'patty@mail.com', 1, '2025-12-19 05:52:07', NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pacientes`
--

CREATE TABLE `pacientes` (
  `id` int(11) NOT NULL,
  `nombres` varchar(100) NOT NULL,
  `apellidos` varchar(100) NOT NULL,
  `documento` varchar(30) NOT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `email` varchar(120) DEFAULT NULL,
  `fecha_nacimiento` date DEFAULT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NULL DEFAULT NULL ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pacientes`
--

INSERT INTO `pacientes` (`id`, `nombres`, `apellidos`, `documento`, `telefono`, `email`, `fecha_nacimiento`, `activo`, `created_at`, `updated_at`) VALUES
(1, 'María José', 'Gómez Silva', '0102030405', '0991234567', 'maria.gomez@gmail.com', '1998-04-12', 1, '2025-12-19 01:21:18', NULL),
(2, 'Luis Alberto', 'Paredes Ruiz', '0912345678', '0987654321', 'luis.paredes@correo.com', '1992-11-03', 1, '2025-12-19 01:21:18', NULL),
(3, 'Ana Paula', 'Cedeño Vera', '1723456789', '0971112233', 'ana.cedeno@correo.com', '1985-07-25', 1, '2025-12-19 01:21:18', NULL),
(4, 'Mateo Alejandro', 'Cevallos Chávez', '2300982349', '123456789', 'matcej04@gmail.com', '2004-12-20', 1, '2025-12-19 05:49:50', NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `reporte_citas_mensual`
--

CREATE TABLE `reporte_citas_mensual` (
  `id` int(11) NOT NULL,
  `anio` int(11) NOT NULL,
  `mes` int(11) NOT NULL,
  `medico_id` int(11) DEFAULT NULL,
  `total_citas` int(11) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `reporte_citas_mensual`
--

INSERT INTO `reporte_citas_mensual` (`id`, `anio`, `mes`, `medico_id`, `total_citas`, `created_at`) VALUES
(1, 2025, 1, 1, 18, '2025-12-19 01:23:21'),
(2, 2025, 1, 2, 22, '2025-12-19 01:23:21'),
(3, 2025, 1, NULL, 55, '2025-12-19 01:23:21');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `citas`
--
ALTER TABLE `citas`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uq_citas_medico_fechahora` (`medico_id`,`fecha_hora`),
  ADD UNIQUE KEY `uq_citas_paciente_fechahora` (`paciente_id`,`fecha_hora`),
  ADD KEY `idx_citas_activo` (`activo`),
  ADD KEY `idx_citas_fecha` (`fecha_hora`),
  ADD KEY `idx_citas_medico_fecha` (`medico_id`,`fecha_hora`),
  ADD KEY `idx_citas_paciente_fecha` (`paciente_id`,`fecha_hora`);

--
-- Indices de la tabla `medicos`
--
ALTER TABLE `medicos`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idx_medicos_activo` (`activo`),
  ADD KEY `idx_medicos_especialidad` (`especialidad`),
  ADD KEY `idx_medicos_apellidos` (`apellidos`);

--
-- Indices de la tabla `pacientes`
--
ALTER TABLE `pacientes`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uq_pacientes_documento` (`documento`),
  ADD KEY `idx_pacientes_activo` (`activo`),
  ADD KEY `idx_pacientes_apellidos` (`apellidos`);

--
-- Indices de la tabla `reporte_citas_mensual`
--
ALTER TABLE `reporte_citas_mensual`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uq_reporte_anio_mes_medico` (`anio`,`mes`,`medico_id`),
  ADD KEY `fk_reporte_medico` (`medico_id`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `citas`
--
ALTER TABLE `citas`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `medicos`
--
ALTER TABLE `medicos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `pacientes`
--
ALTER TABLE `pacientes`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `reporte_citas_mensual`
--
ALTER TABLE `reporte_citas_mensual`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `citas`
--
ALTER TABLE `citas`
  ADD CONSTRAINT `fk_citas_medicos` FOREIGN KEY (`medico_id`) REFERENCES `medicos` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_citas_pacientes` FOREIGN KEY (`paciente_id`) REFERENCES `pacientes` (`id`) ON UPDATE CASCADE;

--
-- Filtros para la tabla `reporte_citas_mensual`
--
ALTER TABLE `reporte_citas_mensual`
  ADD CONSTRAINT `fk_reporte_medico` FOREIGN KEY (`medico_id`) REFERENCES `medicos` (`id`) ON DELETE SET NULL ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
