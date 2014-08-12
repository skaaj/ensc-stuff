-- phpMyAdmin SQL Dump
-- version 4.0.9
-- http://www.phpmyadmin.net
--
-- Client: 127.0.0.1
-- Généré le: Mar 18 Mars 2014 à 21:22
-- Version du serveur: 5.6.14
-- Version de PHP: 5.5.6

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Base de données: `askittest`
--

-- --------------------------------------------------------

--
-- Structure de la table `blacklist`
--

CREATE TABLE IF NOT EXISTS `blacklist` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `email` varchar(128) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MYISAM DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Structure de la table `forgotten_pwd`
--

CREATE TABLE IF NOT EXISTS `forgotten_pwd` (
  `user_email` varchar(128) NOT NULL,
  `sentkey` varchar(128) NOT NULL,
  PRIMARY KEY (`user_email`,`sentkey`)
) ENGINE=MYISAM DEFAULT CHARSET=utf8;

--
-- Contenu de la table `forgotten_pwd`
--

INSERT INTO `forgotten_pwd` (`user_email`, `sentkey`) VALUES
('etienne.3376@gmail.com', '06d44ddf8cf8670e5cbb7930020f71ca'),
('etienne.3376@gmail.com', '092d6085bd98fcfaacc70afa330269bc'),
('etienne.3376@gmail.com', '2dbea29b5fad928af297d04ce5bb37da'),
('etienne.3376@gmail.com', '2f6766c7f24bd712a8f5c0c03cc8faa4'),
('etienne.3376@gmail.com', '443ec620061b895db9fab0dde5cced6b'),
('etienne.3376@gmail.com', '44f9ca0e9b5bee6ab4bcb6de9ee4c368'),
('etienne.3376@gmail.com', '547ddab63133b4558ba76e1314da0a2a'),
('etienne.3376@gmail.com', '5b22361dd2fee48cd214f15cfe135177'),
('etienne.3376@gmail.com', '5e4fccbfcc4a2a30c37e5c96bdb2c08d'),
('etienne.3376@gmail.com', '5f6a96ed4248a52ccd3dcde47acb08ec'),
('etienne.3376@gmail.com', '60058a27de9dae73ffd4c6535116c7b0'),
('etienne.3376@gmail.com', '633c3c860c02a8e61230fbb4c457eb0f'),
('etienne.3376@gmail.com', '657f4faf551203b424e523c28c4246b9'),
('etienne.3376@gmail.com', '9f6a6a39186393eaca4ba2c2edd7372a'),
('etienne.3376@gmail.com', 'a77fc844d5a3bed405ec20375871ee32'),
('etienne.3376@gmail.com', 'b84abf304f7c8859bec78a189243265b'),
('etienne.3376@gmail.com', 'baaeda1f00f5d5ecb696ddba7353f098'),
('etienne.3376@gmail.com', 'bd69377a56096a6b679558ae2f607542'),
('etienne.3376@gmail.com', 'c67171698be6db08b37638dd41aa2364'),
('etienne.3376@gmail.com', 'ca008662b9865c9a11a22ebff9aa21cf'),
('etienne.3376@gmail.com', 'de72cfc2ba18856729c49783bbb23565'),
('etienne.3376@gmail.com', 'e2342635e0a246bda3faec68cbbccd6d'),
('etienne.3376@gmail.com', 'e4b7cbb099a99951c99b78fc35fdd294'),
('etienne.3376@gmail.com', 'e592adc1d0088d346a54781dc27e2daf'),
('etienne.3376@gmail.com', 'f6713527a33dffc0638882a652246d55'),
('etienne.3376@gmail.com', 'fae06af52606196ac954ec8f276b7402');

-- --------------------------------------------------------

--
-- Structure de la table `post`
--

CREATE TABLE IF NOT EXISTS `post` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `message` varchar(4096) NOT NULL,
  `nbvotes` int(11) DEFAULT NULL,
  `date` datetime DEFAULT NULL,
  `solution` tinyint(1) NOT NULL DEFAULT '0',
  `type` int(11) DEFAULT NULL,
  `author` int(11) DEFAULT NULL,
  `parent` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `author` (`author`),
  KEY `type` (`type`),
  KEY `parent` (`parent`),
  KEY `message` (`message`(767)),
  FULLTEXT KEY `full_text_message` (`message`)
) ENGINE=MYISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=68 ;

-- --------------------------------------------------------

--
-- Structure de la table `post_type`
--

CREATE TABLE IF NOT EXISTS `post_type` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MYISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=4 ;

--
-- Contenu de la table `post_type`
--

INSERT INTO `post_type` (`id`, `name`) VALUES
(1, 'user question'),
(2, 'user response'),
(3, 'admin message');

-- --------------------------------------------------------

--
-- Structure de la table `question`
--

CREATE TABLE IF NOT EXISTS `question` (
  `idPost` int(11) NOT NULL DEFAULT '0',
  `title` TEXT DEFAULT NULL,
  `solved` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`idPost`),
  KEY `title` (`title`(767)),
  FULLTEXT KEY `idx_fulltext_title` (`title`)
) ENGINE=MYISAM DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `reference_tag_post`
--

CREATE TABLE IF NOT EXISTS `reference_tag_post` (
  `tagName` varchar(128) NOT NULL DEFAULT '0',
  `postId` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`tagName`,`postId`),
  KEY `postId` (`postId`),
  KEY `tagId` (`tagName`)
) ENGINE=MYISAM DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `report`
--

CREATE TABLE IF NOT EXISTS `report` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `comments` mediumtext,
  `postId` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `postId` (`postId`)
) ENGINE=MYISAM DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Structure de la table `repost`
--

CREATE TABLE IF NOT EXISTS `repost` (
  `idOriginal` int(11) NOT NULL DEFAULT '0',
  `idRepost` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`idOriginal`,`idRepost`),
  KEY `idRepost` (`idRepost`),
  KEY `idOriginal` (`idOriginal`)
) ENGINE=MYISAM DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `role`
--

CREATE TABLE IF NOT EXISTS `role` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MYISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=4 ;

--
-- Contenu de la table `role`
--

INSERT INTO `role` (`id`, `name`) VALUES
(1, 'user'),
(2, 'administrator'),
(3, 'deleted');

-- --------------------------------------------------------

--
-- Structure de la table `save`
--

CREATE TABLE IF NOT EXISTS `save` (
  `userId` int(11) NOT NULL DEFAULT '0',
  `postId` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`userId`,`postId`),
  KEY `userId` (`userId`),
  KEY `postId` (`postId`)
) ENGINE=MYISAM DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `tag`
--

CREATE TABLE IF NOT EXISTS `tag` (
  `name` varchar(128) NOT NULL,
  PRIMARY KEY (`name`)
) ENGINE=MYISAM DEFAULT CHARSET=latin1;

--
-- Contenu de la table `tag`
--

INSERT INTO `tag` (`name`) VALUES
('PHP'),
('PROG'),
('TAG'),
('VTEC'),
('WEB');

-- --------------------------------------------------------

--
-- Structure de la table `user`
--

CREATE TABLE IF NOT EXISTS `user` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `login` varchar(128) CHARACTER SET utf8 NOT NULL,
  `firstname` varchar(128) DEFAULT NULL,
  `lastname` varchar(128) DEFAULT NULL,
  `email` varchar(128) NOT NULL,
  `nbvotes` int(11) DEFAULT NULL,
  `authorized` tinyint(1) NOT NULL,
  `role` int(11) NOT NULL,
  `password` varchar(256) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `email` (`email`),
  UNIQUE KEY `login` (`login`),
  KEY `role` (`role`),
  KEY `email_2` (`email`)
) ENGINE=MYISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=14 ;

--
-- Contenu de la table `user`
--

INSERT INTO `user` (`id`, `login`, `firstname`, `lastname`, `email`, `nbvotes`, `authorized`, `role`, `password`) VALUES
(12, 'DELETEDUSER', 'USER DELETED', 'USER DELETED', 'noemail@askit.com', 0, 1, 3, 'deleted user'),
(13, 'admin', 'admin', 'admin', 'admin@admin.com', 0, 1, 2, 'admin');

-- --------------------------------------------------------

--
-- Structure de la table `vote_user_post`
--

CREATE TABLE IF NOT EXISTS `vote_user_post` (
  `userId` int(11) NOT NULL,
  `postId` int(11) NOT NULL,
  PRIMARY KEY (`userId`,`postId`),
  KEY `userId` (`userId`),
  KEY `postId` (`postId`)
) ENGINE=MYISAM DEFAULT CHARSET=utf8;

--
-- Contraintes pour les tables exportées
--

--
-- Contraintes pour la table `post`
--
ALTER TABLE `post`
  ADD CONSTRAINT `post_ibfk_1` FOREIGN KEY (`author`) REFERENCES `user` (`id`),
  ADD CONSTRAINT `post_ibfk_2` FOREIGN KEY (`type`) REFERENCES `post_type` (`id`),
  ADD CONSTRAINT `post_ibfk_3` FOREIGN KEY (`parent`) REFERENCES `post` (`id`);

--
-- Contraintes pour la table `question`
--
ALTER TABLE `question`
  ADD CONSTRAINT `question_ibfk_1` FOREIGN KEY (`idPost`) REFERENCES `post` (`id`);

--
-- Contraintes pour la table `reference_tag_post`
--
ALTER TABLE `reference_tag_post`
  ADD CONSTRAINT `reference_tag_post_ibfk_1` FOREIGN KEY (`postId`) REFERENCES `post` (`id`),
  ADD CONSTRAINT `reference_tag_post_ibfk_2` FOREIGN KEY (`tagName`) REFERENCES `tag` (`name`);

--
-- Contraintes pour la table `report`
--
ALTER TABLE `report`
  ADD CONSTRAINT `report_ibfk_1` FOREIGN KEY (`postId`) REFERENCES `post` (`id`);

--
-- Contraintes pour la table `repost`
--
ALTER TABLE `repost`
  ADD CONSTRAINT `repost_ibfk_1` FOREIGN KEY (`idOriginal`) REFERENCES `post` (`id`),
  ADD CONSTRAINT `repost_ibfk_2` FOREIGN KEY (`idRepost`) REFERENCES `post` (`id`);

--
-- Contraintes pour la table `save`
--
ALTER TABLE `save`
  ADD CONSTRAINT `save_ibfk_1` FOREIGN KEY (`userId`) REFERENCES `user` (`id`),
  ADD CONSTRAINT `save_ibfk_2` FOREIGN KEY (`postId`) REFERENCES `post` (`id`);

--
-- Contraintes pour la table `user`
--
ALTER TABLE `user`
  ADD CONSTRAINT `user_ibfk_1` FOREIGN KEY (`role`) REFERENCES `role` (`id`);

--
-- Contraintes pour la table `vote_user_post`
--
ALTER TABLE `vote_user_post`
  ADD CONSTRAINT `vote_user_post_ibfk_1` FOREIGN KEY (`userId`) REFERENCES `user` (`id`),
  ADD CONSTRAINT `vote_user_post_ibfk_2` FOREIGN KEY (`postId`) REFERENCES `post` (`id`);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
