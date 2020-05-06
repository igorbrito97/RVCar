-- phpMyAdmin SQL Dump
-- version 5.0.2
-- https://www.phpmyadmin.net/
--
-- Host: localhost
-- Tempo de geração: 06/05/2020 às 19:49
-- Versão do servidor: 10.4.11-MariaDB
-- Versão do PHP: 7.4.4

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Banco de dados: `rvcardb`
--

-- --------------------------------------------------------

--
-- Estrutura para tabela `component`
--

CREATE TABLE `component` (
  `component_id` int(11) NOT NULL,
  `component_name` varchar(60) NOT NULL,
  `component_description` varchar(350) NOT NULL,
  `component_status` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Despejando dados para a tabela `component`
--

INSERT INTO `component` (`component_id`, `component_name`, `component_description`, `component_status`) VALUES
(1, 'Semaforo', 'Semaforo simples ', 1),
(2, 'Estrada de terra', 'Simlua uma estrada rural', 1),
(3, 'Ruas ', 'Rua simples que tem em todo lugar', 1),
(4, 'TesteZao', 'vamos ver', 1),
(5, 'TestezZZIn', 'gogogogogogogogoo', 1);

-- --------------------------------------------------------

--
-- Estrutura para tabela `component_scenario`
--

CREATE TABLE `component_scenario` (
  `component_id` int(11) NOT NULL,
  `scenario_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='componentes que podem ser executados em uma tabela';

--
-- Despejando dados para a tabela `component_scenario`
--

INSERT INTO `component_scenario` (`component_id`, `scenario_id`) VALUES
(1, 1),
(1, 2),
(2, 1),
(2, 2),
(3, 1),
(3, 2),
(3, 3),
(4, 1),
(4, 3),
(5, 3);

-- --------------------------------------------------------

--
-- Estrutura para tabela `patient`
--

CREATE TABLE `patient` (
  `pat_id` int(11) NOT NULL,
  `pat_name` varchar(60) NOT NULL,
  `pat_cpf` varchar(11) NOT NULL,
  `pat_phone` varchar(15) NOT NULL,
  `pat_email` varchar(60) NOT NULL,
  `pat_note` varchar(250) NOT NULL,
  `pat_gender` char(1) NOT NULL,
  `pat_birthday` date NOT NULL,
  `pat_status` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Despejando dados para a tabela `patient`
--

INSERT INTO `patient` (`pat_id`, `pat_name`, `pat_cpf`, `pat_phone`, `pat_email`, `pat_note`, `pat_gender`, `pat_birthday`, `pat_status`) VALUES
(1, 'Igor', '123456', '25697', 'emai@.com', 'paciente novo do bd novo', 'F', '2000-01-01', 1),
(2, 'Iguinho', '111', '123123', 'eaae@asd', '', 'M', '1968-01-01', 0),
(3, 'Pacitizo', '123', '00002', 'eme.com', 'observation', 'M', '1987-01-01', 0),
(4, 'Stella', '12313', '123123', 'stel.com', 'paciente fera demais', 'F', '2001-04-12', 1);

-- --------------------------------------------------------

--
-- Estrutura para tabela `psychologist`
--

CREATE TABLE `psychologist` (
  `psyc_id` int(11) NOT NULL,
  `psyc_name` varchar(45) NOT NULL,
  `psyc_cpf` varchar(11) NOT NULL,
  `psyc_email` varchar(60) NOT NULL,
  `psyc_phone` varchar(15) NOT NULL,
  `psyc_birthday` date NOT NULL,
  `psyc_gender` char(1) NOT NULL,
  `psyc_crp` varchar(45) NOT NULL,
  `psyc_status` tinyint(4) NOT NULL,
  `psyc_password` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Despejando dados para a tabela `psychologist`
--

INSERT INTO `psychologist` (`psyc_id`, `psyc_name`, `psyc_cpf`, `psyc_email`, `psyc_phone`, `psyc_birthday`, `psyc_gender`, `psyc_crp`, `psyc_status`, `psyc_password`) VALUES
(1, 'Igor', '456123', 'email@.com', '123', '1998-01-01', 'M', '123', 1, 'senha'),
(2, 'Stellinha', '555', 'email@igor.com', '555', '2001-04-12', 'F', '123', 1, 'igor123'),
(3, 'Novo Psic Do Trabalho', '111111', 'email@email.com', '15334', '1965-06-24', 'F', '25896', 0, 'senha'),
(4, 'Psicologu', '123', 'eme@eme.com.br', '1234598730', '1997-12-30', 'F', '123456789', 1, '123');

-- --------------------------------------------------------

--
-- Estrutura para tabela `scenario`
--

CREATE TABLE `scenario` (
  `scenario_id` int(11) NOT NULL,
  `scenario_name` varchar(60) NOT NULL,
  `scenario_description` varchar(350) NOT NULL,
  `scenario_status` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Despejando dados para a tabela `scenario`
--

INSERT INTO `scenario` (`scenario_id`, `scenario_name`, `scenario_description`, `scenario_status`) VALUES
(1, 'Teste', 'Cenario inicial', 1),
(2, 'Rural', 'Simular uma estrada no meio rural', 1),
(3, 'Residencial', 'casas e mais caasas', 1);

-- --------------------------------------------------------

--
-- Estrutura para tabela `session`
--

CREATE TABLE `session` (
  `session_id` int(11) NOT NULL,
  `psychologist_id` int(11) NOT NULL,
  `patient_id` int(11) NOT NULL,
  `stage_id` int(11) NOT NULL,
  `session_name` varchar(60) NOT NULL,
  `session_description` varchar(450) NOT NULL,
  `session_status` tinyint(4) NOT NULL,
  `session_public` tinyint(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Despejando dados para a tabela `session`
--

INSERT INTO `session` (`session_id`, `psychologist_id`, `patient_id`, `stage_id`, `session_name`, `session_description`, `session_status`, `session_public`) VALUES
(1, 1, 4, 3, 'sesss', 'descptttt', 1, 1);

-- --------------------------------------------------------

--
-- Estrutura para tabela `stage`
--

CREATE TABLE `stage` (
  `stage_id` int(11) NOT NULL,
  `scenario_id` int(11) NOT NULL,
  `weather_id` int(11) NOT NULL,
  `stage_description` varchar(350) NOT NULL,
  `stage_time` int(11) NOT NULL,
  `stage_status` tinyint(1) NOT NULL,
  `stage_name` varchar(60) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Despejando dados para a tabela `stage`
--

INSERT INTO `stage` (`stage_id`, `scenario_id`, `weather_id`, `stage_description`, `stage_time`, `stage_status`, `stage_name`) VALUES
(3, 3, 1, 'decricao sessao \nagora ja foi fio\nsucesso', 20, 1, 'sessao pronta'),
(4, 2, 2, 'cena para se ambientar a ambientes rurais com muita ilumiacao', 12, 1, 'cena rural'),
(5, 3, 2, 'description do stage gogo', 2, 0, 'Stage');

-- --------------------------------------------------------

--
-- Estrutura para tabela `stage_component`
--

CREATE TABLE `stage_component` (
  `stage_id` int(11) NOT NULL,
  `component_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Despejando dados para a tabela `stage_component`
--

INSERT INTO `stage_component` (`stage_id`, `component_id`) VALUES
(3, 5),
(4, 1),
(4, 2),
(5, 3),
(5, 4);

-- --------------------------------------------------------

--
-- Estrutura para tabela `weather`
--

CREATE TABLE `weather` (
  `weather_id` int(11) NOT NULL,
  `weather_name` varchar(60) NOT NULL,
  `weather_description` varchar(350) NOT NULL,
  `weather_status` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Despejando dados para a tabela `weather`
--

INSERT INTO `weather` (`weather_id`, `weather_name`, `weather_description`, `weather_status`) VALUES
(1, 'Sol', 'Verao zao fera sol churrasco cerveja', 1),
(2, 'Chuva', 'Chuvas e pingos de alegria', 1);

--
-- Índices de tabelas apagadas
--

--
-- Índices de tabela `component`
--
ALTER TABLE `component`
  ADD PRIMARY KEY (`component_id`);

--
-- Índices de tabela `component_scenario`
--
ALTER TABLE `component_scenario`
  ADD PRIMARY KEY (`component_id`,`scenario_id`),
  ADD KEY `fk_scenario_id` (`scenario_id`),
  ADD KEY `fk_component_id` (`component_id`) USING BTREE;

--
-- Índices de tabela `patient`
--
ALTER TABLE `patient`
  ADD PRIMARY KEY (`pat_id`),
  ADD UNIQUE KEY `unique_pat_cpf` (`pat_cpf`) USING BTREE;

--
-- Índices de tabela `psychologist`
--
ALTER TABLE `psychologist`
  ADD PRIMARY KEY (`psyc_id`),
  ADD UNIQUE KEY `unique_psyc_cpf` (`psyc_cpf`);

--
-- Índices de tabela `scenario`
--
ALTER TABLE `scenario`
  ADD PRIMARY KEY (`scenario_id`);

--
-- Índices de tabela `session`
--
ALTER TABLE `session`
  ADD PRIMARY KEY (`session_id`),
  ADD KEY `fk_psyc` (`psychologist_id`),
  ADD KEY `fk_patient` (`patient_id`),
  ADD KEY `fk_stage` (`stage_id`);

--
-- Índices de tabela `stage`
--
ALTER TABLE `stage`
  ADD PRIMARY KEY (`stage_id`),
  ADD KEY `fk_stage_scenario` (`scenario_id`) USING BTREE,
  ADD KEY `fk_stage_weather` (`weather_id`) USING BTREE;

--
-- Índices de tabela `stage_component`
--
ALTER TABLE `stage_component`
  ADD PRIMARY KEY (`stage_id`,`component_id`) USING BTREE;

--
-- Índices de tabela `weather`
--
ALTER TABLE `weather`
  ADD PRIMARY KEY (`weather_id`);

--
-- AUTO_INCREMENT de tabelas apagadas
--

--
-- AUTO_INCREMENT de tabela `component`
--
ALTER TABLE `component`
  MODIFY `component_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de tabela `patient`
--
ALTER TABLE `patient`
  MODIFY `pat_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de tabela `psychologist`
--
ALTER TABLE `psychologist`
  MODIFY `psyc_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de tabela `scenario`
--
ALTER TABLE `scenario`
  MODIFY `scenario_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de tabela `session`
--
ALTER TABLE `session`
  MODIFY `session_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de tabela `stage`
--
ALTER TABLE `stage`
  MODIFY `stage_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de tabela `weather`
--
ALTER TABLE `weather`
  MODIFY `weather_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- Restrições para dumps de tabelas
--

--
-- Restrições para tabelas `component_scenario`
--
ALTER TABLE `component_scenario`
  ADD CONSTRAINT `fk_component` FOREIGN KEY (`component_id`) REFERENCES `component` (`component_id`),
  ADD CONSTRAINT `fk_component_id` FOREIGN KEY (`component_id`) REFERENCES `component` (`component_id`),
  ADD CONSTRAINT `fk_scenario_id` FOREIGN KEY (`scenario_id`) REFERENCES `scenario` (`scenario_id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
