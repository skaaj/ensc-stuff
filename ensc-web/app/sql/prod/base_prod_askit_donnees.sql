-- phpMyAdmin SQL Dump
-- version 4.0.9
-- http://www.phpmyadmin.net
--
-- Client: 127.0.0.1
-- Généré le: Jeu 27 Mars 2014 à 11:19
-- Version du serveur: 5.6.14
-- Version de PHP: 5.5.6

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Base de données: `askit`
--



--
-- Contenu de la table `post_type`
--

INSERT INTO `post_type` (`id`, `name`) VALUES
(1, 'user question'),
(2, 'user response'),
(3, 'admin message');

--
-- Contenu de la table `role`
--

INSERT INTO `role` (`id`, `name`) VALUES
(1, 'user'),
(2, 'administrator'),
(3, 'deleted');

--
-- Contenu de la table `tag`
--

INSERT INTO `tag` (`name`) VALUES
('Bricolage'),
('Cuisine'),
('gros'),
('honda'),
('MÃ©canique'),
('Mécanique'),
('Programmation'),
('Sciences'),
('Voiture'),
('Voyage'),
('web');

--
-- Contenu de la table `user`
--

INSERT INTO `user` (`id`, `login`, `firstname`, `lastname`, `email`, `nbvotes`, `authorized`, `role`, `password`) VALUES
(12, 'Deleted user', 'Deleted user', 'Deleted user', 'noemail@askit.com', 0, 0, 3, 'deleted user'),
(13, 'admin', 'admin', 'admin', 'admin@admin.com', 3, 1, 2, 'admin');

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
