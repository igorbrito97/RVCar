-- phpMyAdmin SQL Dump
-- version 4.9.0.1
-- https://www.phpmyadmin.net/
--
-- Host: localhost
-- Tempo de geração: 02-Jul-2020 às 07:22
-- Versão do servidor: 10.4.6-MariaDB
-- versão do PHP: 7.1.32

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
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
-- Estrutura da tabela `component`
--

CREATE TABLE `component` (
  `component_id` int(11) NOT NULL,
  `component_name` varchar(60) NOT NULL,
  `component_description` varchar(250) NOT NULL,
  `component_status` tinyint(4) NOT NULL,
  `objComp_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Extraindo dados da tabela `component`
--

INSERT INTO `component` (`component_id`, `component_name`, `component_description`, `component_status`, `objComp_id`) VALUES
(1, '', '', 0, 0),
(2, 'Obstaculo', 'Tartaruga', 1, 0),
(3, 'Semaforo', 'Semarf', 1, 0),
(4, 'Transito', 'Transss', 1, 0),
(5, 'Automóvel', 'Componente referente aos carros que ficam movimentando ', 1, 1),
(6, 'Garagem', 'Esse componente permite que o paciente possa estacionar o carro', 1, 2);

-- --------------------------------------------------------

--
-- Estrutura da tabela `component_scenario`
--

CREATE TABLE `component_scenario` (
  `scenario_id` int(11) NOT NULL,
  `component_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Extraindo dados da tabela `component_scenario`
--

INSERT INTO `component_scenario` (`scenario_id`, `component_id`) VALUES
(3, 5),
(5, 5),
(5, 6),
(6, 5),
(7, 5),
(7, 6),
(8, 5);

-- --------------------------------------------------------

--
-- Estrutura da tabela `compsce_quantity`
--

CREATE TABLE `compsce_quantity` (
  `comp_id` int(11) NOT NULL,
  `sce_id` int(11) NOT NULL,
  `quantmax` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Extraindo dados da tabela `compsce_quantity`
--

INSERT INTO `compsce_quantity` (`comp_id`, `sce_id`, `quantmax`) VALUES
(5, 3, 2),
(5, 7, 5),
(5, 5, 10),
(5, 6, 15),
(5, 8, 12);

-- --------------------------------------------------------

--
-- Estrutura da tabela `environmenttype`
--

CREATE TABLE `environmenttype` (
  `env_id` int(11) NOT NULL,
  `env_name` varchar(60) NOT NULL,
  `env_description` varchar(350) NOT NULL,
  `env_status` tinyint(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Extraindo dados da tabela `environmenttype`
--

INSERT INTO `environmenttype` (`env_id`, `env_name`, `env_description`, `env_status`) VALUES
(1, 'Urbano', 'Simulaçao de ambientes dentro de um perimetro urbano', 1),
(2, 'Garagem', 'Cenario com uma garagem simples para entrar e sair', 1),
(3, 'Rural', 'Cenario para estradas de terra e campos afastados da cidade', 1),
(4, 'Teste', 'Cenarios para teste, bem simples e com poucos elementos facilitando com que o paciente possa se familiarizar com a plataforma', 1);

-- --------------------------------------------------------

--
-- Estrutura da tabela `objcar`
--

CREATE TABLE `objcar` (
  `objCar_id` int(11) NOT NULL,
  `objCar_name` varchar(60) NOT NULL,
  `objCar_file` varchar(100) NOT NULL,
  `objCar_prefab` varchar(60) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Extraindo dados da tabela `objcar`
--

INSERT INTO `objcar` (`objCar_id`, `objCar_name`, `objCar_file`, `objCar_prefab`) VALUES
(1, 'Esportivo', 'CarImage/SportCoupe', 'SportCoupe'),
(2, 'Pickup 1', 'CarImage/Pickup1', 'Pickup1'),
(3, 'Pickup 2', 'CarImage/Pickup2', 'Pickup2');

-- --------------------------------------------------------

--
-- Estrutura da tabela `objcomponent`
--

CREATE TABLE `objcomponent` (
  `objComp_id` int(11) NOT NULL,
  `objComp_name` varchar(60) NOT NULL,
  `objComp_file` varchar(100) NOT NULL,
  `objComp_prefab` varchar(60) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Extraindo dados da tabela `objcomponent`
--

INSERT INTO `objcomponent` (`objComp_id`, `objComp_name`, `objComp_file`, `objComp_prefab`) VALUES
(1, 'Carros', 'ComponentImage/Car', 'Car'),
(2, 'Garagem', 'ComponentImage/Garage', 'Garage');

-- --------------------------------------------------------

--
-- Estrutura da tabela `objscenario`
--

CREATE TABLE `objscenario` (
  `objSce_id` int(11) NOT NULL,
  `objSce_name` varchar(60) NOT NULL,
  `objSce_file` varchar(100) NOT NULL,
  `objSce_prefab` varchar(60) NOT NULL,
  `objSce_carpos` varchar(50) NOT NULL,
  `objSce_garagepos` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Extraindo dados da tabela `objscenario`
--

INSERT INTO `objscenario` (`objSce_id`, `objSce_name`, `objSce_file`, `objSce_prefab`, `objSce_carpos`, `objSce_garagepos`) VALUES
(1, 'Garagem Teste', 'ScenarioImage/GarageTest', 'GarageTest', '70,1,-91.4,0,270,0', ''),
(2, 'Cidade Teste', 'ScenarioImage/CityTest', 'CityTest', '6.7,0.1,-240.58,0,0,0', '71,2.5,-266.7,0,0,0'),
(5, 'Cidade', 'ScenarioImage/City', 'City', '107,0.91,-79,0.7,93,-2', '-169,3.4,22,0,-96,0'),
(6, 'Cidade com rodovia', 'ScenarioImage/CityRoad', 'CityRoad', '-150,0.71,-48,0,274,0', ''),
(7, 'Cidade sem rodovia', 'ScenarioImage/CityCenter', 'CityCenter', '-181.6,0.7,-33,0,173,0', '');

-- --------------------------------------------------------

--
-- Estrutura da tabela `patient`
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
-- Extraindo dados da tabela `patient`
--

INSERT INTO `patient` (`pat_id`, `pat_name`, `pat_cpf`, `pat_phone`, `pat_email`, `pat_note`, `pat_gender`, `pat_birthday`, `pat_status`) VALUES
(1, 'Igor', '123456', '25697', 'emai@.com', 'paciente novo do bd novo', 'F', '2000-01-01', 1),
(2, 'Iguinho', '111', '123123', 'eaae@asd', '', 'M', '1968-01-01', 0),
(3, 'Pacitizo', '123', '00002', 'eme.com', 'observation', 'M', '1987-01-01', 0),
(4, 'Stella', '12313', '123123', 'stel.com', 'paciente fera demais', 'F', '2001-04-12', 1),
(5, 'Mensageiro', '123123123', '222222', 'msg@email.com', 'aaa', 'M', '1991-01-01', 1);

-- --------------------------------------------------------

--
-- Estrutura da tabela `psychologist`
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
-- Extraindo dados da tabela `psychologist`
--

INSERT INTO `psychologist` (`psyc_id`, `psyc_name`, `psyc_cpf`, `psyc_email`, `psyc_phone`, `psyc_birthday`, `psyc_gender`, `psyc_crp`, `psyc_status`, `psyc_password`) VALUES
(1, 'Igor', '456123', 'email@.com', '123', '1998-01-01', 'M', '123', 1, 'senha'),
(2, 'Stellinha', '555', 'email@igor.com', '555', '2001-04-12', 'F', '123', 1, 'igor123'),
(3, 'Novo Psic Do Trabalho', '111111', 'email@email.com', '15334', '1965-06-24', 'F', '25896', 0, 'senha'),
(4, 'Psicologu', '123', 'eme@eme.com.br', '1234598730', '1997-12-30', 'F', '123456789', 1, '123');

-- --------------------------------------------------------

--
-- Estrutura da tabela `scenario`
--

CREATE TABLE `scenario` (
  `scenario_id` int(11) NOT NULL,
  `scenario_name` varchar(60) NOT NULL,
  `scenario_description` varchar(250) NOT NULL,
  `env_id` int(11) NOT NULL,
  `scenario_status` tinyint(4) NOT NULL,
  `objSce_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Extraindo dados da tabela `scenario`
--

INSERT INTO `scenario` (`scenario_id`, `scenario_name`, `scenario_description`, `env_id`, `scenario_status`, `objSce_id`) VALUES
(1, 'Centro', 'Simulaçao do centro de uma cidade', 1, 0, 2),
(2, 'Rural', 'Rural', 3, 0, 0),
(3, 'Garagem Simples', 'Cenário que simula uma rua oval com somente uma garagem. É possível adicionar carros andando', 2, 1, 1),
(4, 'Garagem dificil', 'alo alo', 2, 0, 1),
(5, 'Cidade principal', 'Cidade com construções, ruas e rodovia. Possibilidade de adicionar carros se movimentando e garagem', 1, 1, 5),
(6, 'Simulação de rodovias', 'Nesse cenário o paciente é posto perto da entrada para um rodovia. É possível de se adicionar carros', 1, 1, 6),
(7, 'Cidade simples', 'Esse cenário simula um pequeno bairro sem muita moviemtação.Existe a possibilidade de se adicionar carros e garagem', 4, 1, 2),
(8, 'Área urbana', 'Simulação de uma área com casas e prédios', 1, 1, 7);

-- --------------------------------------------------------

--
-- Estrutura da tabela `session`
--

CREATE TABLE `session` (
  `session_id` int(11) NOT NULL,
  `psychologist_id` int(11) NOT NULL,
  `patient_id` int(11) NOT NULL,
  `weather_id` int(11) NOT NULL,
  `scenario_id` int(11) NOT NULL,
  `session_name` varchar(60) NOT NULL,
  `session_description` varchar(250) NOT NULL,
  `session_status` tinyint(4) NOT NULL,
  `session_public` tinyint(4) NOT NULL,
  `car_id` int(11) NOT NULL,
  `session_gear` tinyint(4) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Extraindo dados da tabela `session`
--

INSERT INTO `session` (`session_id`, `psychologist_id`, `patient_id`, `weather_id`, `scenario_id`, `session_name`, `session_description`, `session_status`, `session_public`, `car_id`, `session_gear`) VALUES
(1, 1, 1, 3, 1, 'Primeira sessao', 'Sessao numero 1 do programa', 0, 1, 1, 0),
(2, 1, 4, 3, 4, 'Segunda sessao', 'Agora vamos de segundona', 0, 1, 2, 0),
(3, 1, 4, 4, 6, 'Testezaaaa', '1231231231231', 1, 0, 1, 1),
(4, 1, 1, 3, 7, 'qqq', 'qqq', 0, 1, 3, 1),
(5, 1, 1, 4, 3, 'Sessão garagT', '22', 1, 1, 2, 0),
(6, 1, 5, 3, 7, 'CityT', '3', 1, 1, 3, 0),
(7, 1, 4, 4, 5, 'Cityzda', '1', 1, 1, 1, 0),
(8, 1, 5, 3, 8, 'Cter', 'a', 1, 1, 2, 0);

-- --------------------------------------------------------

--
-- Estrutura da tabela `session_component`
--

CREATE TABLE `session_component` (
  `session_id` int(11) NOT NULL,
  `component_id` int(11) NOT NULL,
  `sescomp_quantity` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Extraindo dados da tabela `session_component`
--

INSERT INTO `session_component` (`session_id`, `component_id`, `sescomp_quantity`) VALUES
(2, 3, 1),
(2, 4, 1),
(3, 5, 15),
(3, 6, 1),
(6, 6, 1),
(7, 6, 1);

-- --------------------------------------------------------

--
-- Estrutura da tabela `weather`
--

CREATE TABLE `weather` (
  `weather_id` int(11) NOT NULL,
  `weather_name` varchar(60) NOT NULL,
  `weather_description` varchar(350) NOT NULL,
  `weather_status` tinyint(1) NOT NULL,
  `weatherType_id` int(11) NOT NULL,
  `weather_info` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Extraindo dados da tabela `weather`
--

INSERT INTO `weather` (`weather_id`, `weather_name`, `weather_description`, `weather_status`, `weatherType_id`, `weather_info`) VALUES
(3, 'Chuva leve', 'Pequena quantidade de chuva', 1, 2, 100),
(4, 'Dia nublado', 'Dia muito nublado, dificultando a visao', 1, 4, 800);

-- --------------------------------------------------------

--
-- Estrutura da tabela `weathertype`
--

CREATE TABLE `weathertype` (
  `weatherType_id` int(11) NOT NULL,
  `weatherType_name` varchar(60) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Extraindo dados da tabela `weathertype`
--

INSERT INTO `weathertype` (`weatherType_id`, `weatherType_name`) VALUES
(1, 'Sol'),
(2, 'Chuva'),
(3, 'Tempestade'),
(4, 'Nublado');

--
-- Índices para tabelas despejadas
--

--
-- Índices para tabela `component`
--
ALTER TABLE `component`
  ADD PRIMARY KEY (`component_id`);

--
-- Índices para tabela `component_scenario`
--
ALTER TABLE `component_scenario`
  ADD PRIMARY KEY (`scenario_id`,`component_id`);

--
-- Índices para tabela `environmenttype`
--
ALTER TABLE `environmenttype`
  ADD PRIMARY KEY (`env_id`);

--
-- Índices para tabela `objcar`
--
ALTER TABLE `objcar`
  ADD PRIMARY KEY (`objCar_id`);

--
-- Índices para tabela `objcomponent`
--
ALTER TABLE `objcomponent`
  ADD PRIMARY KEY (`objComp_id`);

--
-- Índices para tabela `objscenario`
--
ALTER TABLE `objscenario`
  ADD PRIMARY KEY (`objSce_id`);

--
-- Índices para tabela `patient`
--
ALTER TABLE `patient`
  ADD PRIMARY KEY (`pat_id`),
  ADD UNIQUE KEY `unique_pat_cpf` (`pat_cpf`) USING BTREE;

--
-- Índices para tabela `psychologist`
--
ALTER TABLE `psychologist`
  ADD PRIMARY KEY (`psyc_id`),
  ADD UNIQUE KEY `unique_psyc_cpf` (`psyc_cpf`);

--
-- Índices para tabela `scenario`
--
ALTER TABLE `scenario`
  ADD PRIMARY KEY (`scenario_id`);

--
-- Índices para tabela `session`
--
ALTER TABLE `session`
  ADD PRIMARY KEY (`session_id`);

--
-- Índices para tabela `session_component`
--
ALTER TABLE `session_component`
  ADD PRIMARY KEY (`session_id`,`component_id`);

--
-- Índices para tabela `weather`
--
ALTER TABLE `weather`
  ADD PRIMARY KEY (`weather_id`);

--
-- Índices para tabela `weathertype`
--
ALTER TABLE `weathertype`
  ADD PRIMARY KEY (`weatherType_id`);

--
-- AUTO_INCREMENT de tabelas despejadas
--

--
-- AUTO_INCREMENT de tabela `component`
--
ALTER TABLE `component`
  MODIFY `component_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT de tabela `environmenttype`
--
ALTER TABLE `environmenttype`
  MODIFY `env_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de tabela `objcar`
--
ALTER TABLE `objcar`
  MODIFY `objCar_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de tabela `objcomponent`
--
ALTER TABLE `objcomponent`
  MODIFY `objComp_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de tabela `objscenario`
--
ALTER TABLE `objscenario`
  MODIFY `objSce_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT de tabela `patient`
--
ALTER TABLE `patient`
  MODIFY `pat_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de tabela `psychologist`
--
ALTER TABLE `psychologist`
  MODIFY `psyc_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de tabela `scenario`
--
ALTER TABLE `scenario`
  MODIFY `scenario_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT de tabela `session`
--
ALTER TABLE `session`
  MODIFY `session_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT de tabela `weather`
--
ALTER TABLE `weather`
  MODIFY `weather_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de tabela `weathertype`
--
ALTER TABLE `weathertype`
  MODIFY `weatherType_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
