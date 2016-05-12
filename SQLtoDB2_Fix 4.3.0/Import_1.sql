/*
MySQL Data Transfer

Source Server         : localhost
Source Server Version : 50150
Source Host           : localhost:3306
Source Database       : db2

Target Server Type    : MYSQL
Target Server Version : 50150
File Encoding         : 65001
Date: 2011-09-11 20:07:07
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for `dbc_item`
-- ----------------------------
DROP TABLE IF EXISTS `dbc_item`;
CREATE TABLE `dbc_item` (
  `id` int(10) unsigned NOT NULL DEFAULT '0',
  `class` int(10) unsigned NOT NULL DEFAULT '0',
  `subclass` int(10) unsigned NOT NULL DEFAULT '0',
  `unk0` int(11) NOT NULL DEFAULT '0',
  `material` int(11) NOT NULL DEFAULT '0',
  `displayid` int(10) unsigned NOT NULL DEFAULT '0',
  `inventorytype` int(10) unsigned NOT NULL DEFAULT '0',
  `sheath` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='Export of Item.db2';

-- ----------------------------
-- Records of dbc_item
-- ----------------------------

-- ----------------------------
-- Table structure for `dbc_item-sparse`
-- ----------------------------
DROP TABLE IF EXISTS `dbc_item-sparse`;
CREATE TABLE `dbc_item-sparse` (
  `id` int(11) NOT NULL DEFAULT '0',
  `quality` int(11) NOT NULL DEFAULT '0',
  `flags` int(10) unsigned NOT NULL DEFAULT '0',
  `flags2` int(10) unsigned NOT NULL DEFAULT '0',
  `buyprice` int(11) NOT NULL DEFAULT '0',
  `sellprice` int(11) NOT NULL DEFAULT '0',
  `inventorytype` int(11) NOT NULL DEFAULT '0',
  `allowableclass` int(11) NOT NULL DEFAULT '0',
  `allowablerace` int(11) NOT NULL DEFAULT '0',
  `itemlevel` int(11) NOT NULL DEFAULT '0',
  `requiredlevel` int(11) NOT NULL DEFAULT '0',
  `requiredskill` int(11) NOT NULL DEFAULT '0',
  `requiredskillrank` int(11) NOT NULL DEFAULT '0',
  `requiredspell` int(11) NOT NULL DEFAULT '0',
  `requiredhonorrank` int(11) NOT NULL DEFAULT '0',
  `requiredcityrank` int(11) NOT NULL DEFAULT '0',
  `requiredreputationfaction` int(11) NOT NULL DEFAULT '0',
  `requiredreputationrank` int(11) NOT NULL DEFAULT '0',
  `maxcount` int(11) NOT NULL DEFAULT '0',
  `stackable` int(11) NOT NULL DEFAULT '0',
  `containerslots` int(11) NOT NULL DEFAULT '0',
  `stat_type1` int(11) NOT NULL DEFAULT '0',
  `stat_type2` int(11) NOT NULL DEFAULT '0',
  `stat_type3` int(11) NOT NULL DEFAULT '0',
  `stat_type4` int(11) NOT NULL DEFAULT '0',
  `stat_type5` int(11) NOT NULL DEFAULT '0',
  `stat_type6` int(11) NOT NULL DEFAULT '0',
  `stat_type7` int(11) NOT NULL DEFAULT '0',
  `stat_type8` int(11) NOT NULL DEFAULT '0',
  `stat_type9` int(11) NOT NULL DEFAULT '0',
  `stat_type10` int(11) NOT NULL DEFAULT '0',
  `stat_value1` int(11) NOT NULL DEFAULT '0',
  `stat_value2` int(11) NOT NULL DEFAULT '0',
  `stat_value3` int(11) NOT NULL DEFAULT '0',
  `stat_value4` int(11) NOT NULL DEFAULT '0',
  `stat_value5` int(11) NOT NULL DEFAULT '0',
  `stat_value6` int(11) NOT NULL DEFAULT '0',
  `stat_value7` int(11) NOT NULL DEFAULT '0',
  `stat_value8` int(11) NOT NULL DEFAULT '0',
  `stat_value9` int(11) NOT NULL DEFAULT '0',
  `stat_value10` int(11) NOT NULL DEFAULT '0',
  `stat_unk1_1` int(11) NOT NULL DEFAULT '0',
  `stat_unk1_2` int(11) NOT NULL DEFAULT '0',
  `stat_unk1_3` int(11) NOT NULL DEFAULT '0',
  `stat_unk1_4` int(11) NOT NULL DEFAULT '0',
  `stat_unk1_5` int(11) NOT NULL DEFAULT '0',
  `stat_unk1_6` int(11) NOT NULL DEFAULT '0',
  `stat_unk1_7` int(11) NOT NULL DEFAULT '0',
  `stat_unk1_8` int(11) NOT NULL DEFAULT '0',
  `stat_unk1_9` int(11) NOT NULL DEFAULT '0',
  `stat_unk1_10` int(11) NOT NULL DEFAULT '0',
  `stat_unk2_1` int(11) NOT NULL DEFAULT '0',
  `stat_unk2_2` int(11) NOT NULL DEFAULT '0',
  `stat_unk2_3` int(11) NOT NULL DEFAULT '0',
  `stat_unk2_4` int(11) NOT NULL DEFAULT '0',
  `stat_unk2_5` int(11) NOT NULL DEFAULT '0',
  `stat_unk2_6` int(11) NOT NULL DEFAULT '0',
  `stat_unk2_7` int(11) NOT NULL DEFAULT '0',
  `stat_unk2_8` int(11) NOT NULL DEFAULT '0',
  `stat_unk2_9` int(11) NOT NULL DEFAULT '0',
  `stat_unk2_10` int(11) NOT NULL DEFAULT '0',
  `scalingstatdistribution` int(11) NOT NULL DEFAULT '0',
  `damagetype` int(11) NOT NULL DEFAULT '0',
  `delay` int(11) NOT NULL DEFAULT '0',
  `rangedmodrange` float NOT NULL DEFAULT '0',
  `spellid_1` int(11) NOT NULL DEFAULT '0',
  `spellid_2` int(11) NOT NULL DEFAULT '0',
  `spellid_3` int(11) NOT NULL DEFAULT '0',
  `spellid_4` int(11) NOT NULL DEFAULT '0',
  `spellid_5` int(11) NOT NULL DEFAULT '0',
  `spelltrigger_1` int(11) NOT NULL DEFAULT '0',
  `spelltrigger_2` int(11) NOT NULL DEFAULT '0',
  `spelltrigger_3` int(11) NOT NULL DEFAULT '0',
  `spelltrigger_4` int(11) NOT NULL DEFAULT '0',
  `spelltrigger_5` int(11) NOT NULL DEFAULT '0',
  `spellcharges_1` int(11) NOT NULL DEFAULT '0',
  `spellcharges_2` int(11) NOT NULL DEFAULT '0',
  `spellcharges_3` int(11) NOT NULL DEFAULT '0',
  `spellcharges_4` int(11) NOT NULL DEFAULT '0',
  `spellcharges_5` int(11) NOT NULL DEFAULT '0',
  `spellcooldown_1` int(11) NOT NULL DEFAULT '0',
  `spellcooldown_2` int(11) NOT NULL DEFAULT '0',
  `spellcooldown_3` int(11) NOT NULL DEFAULT '0',
  `spellcooldown_4` int(11) NOT NULL DEFAULT '0',
  `spellcooldown_5` int(11) NOT NULL DEFAULT '0',
  `spellcategory_1` int(11) NOT NULL DEFAULT '0',
  `spellcategory_2` int(11) NOT NULL DEFAULT '0',
  `spellcategory_3` int(11) NOT NULL DEFAULT '0',
  `spellcategory_4` int(11) NOT NULL DEFAULT '0',
  `spellcategory_5` int(11) NOT NULL DEFAULT '0',
  `spellcategorycooldown_1` int(11) NOT NULL DEFAULT '0',
  `spellcategorycooldown_2` int(11) NOT NULL DEFAULT '0',
  `spellcategorycooldown_3` int(11) NOT NULL DEFAULT '0',
  `spellcategorycooldown_4` int(11) NOT NULL DEFAULT '0',
  `spellcategorycooldown_5` int(11) NOT NULL DEFAULT '0',
  `bonding` int(11) NOT NULL DEFAULT '0',
  `name` text NOT NULL,
  `name2` text NOT NULL,
  `name3` text NOT NULL,
  `name4` text NOT NULL,
  `description` text NOT NULL,
  `pagetext` int(11) NOT NULL DEFAULT '0',
  `languageid` int(11) NOT NULL DEFAULT '0',
  `pagematerial` int(11) NOT NULL DEFAULT '0',
  `startquest` int(11) NOT NULL DEFAULT '0',
  `lockid` int(11) NOT NULL DEFAULT '0',
  `material` int(11) NOT NULL DEFAULT '0',
  `sheath` int(11) NOT NULL DEFAULT '0',
  `randomproperty` int(11) NOT NULL DEFAULT '0',
  `randomsuffix` int(11) NOT NULL DEFAULT '0',
  `itemset` int(11) NOT NULL DEFAULT '0',
  `maxdurability` int(11) NOT NULL DEFAULT '0',
  `area` int(11) NOT NULL DEFAULT '0',
  `map` int(11) NOT NULL DEFAULT '0',
  `bagfamily` int(11) NOT NULL DEFAULT '0',
  `totemcategory` int(11) NOT NULL DEFAULT '0',
  `socketcolor_1` int(11) NOT NULL DEFAULT '0',
  `socketcolor_2` int(11) NOT NULL DEFAULT '0',
  `socketcolor_3` int(11) NOT NULL DEFAULT '0',
  `socketcontent_1` int(11) NOT NULL DEFAULT '0',
  `socketcontent_2` int(11) NOT NULL DEFAULT '0',
  `socketcontent_3` int(11) NOT NULL DEFAULT '0',
  `socketbonus` int(11) NOT NULL DEFAULT '0',
  `gemproperties` int(11) NOT NULL DEFAULT '0',
  `armordamagemodifier` float NOT NULL DEFAULT '0',
  `duration` int(11) NOT NULL DEFAULT '0',
  `itemlimitcategory` int(11) NOT NULL DEFAULT '0',
  `holidayid` int(11) NOT NULL DEFAULT '0',
  `statscalingfactor` float NOT NULL DEFAULT '0',
  `field_130` int(11) NOT NULL DEFAULT '0',
  `field_131` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='Export of Item-sparse.db2';

-- ----------------------------
-- Records of dbc_item-sparse
-- ----------------------------