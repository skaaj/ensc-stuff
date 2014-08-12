<?php

namespace Data;
abstract class OrderPolicy{
	const ASCENDING="ASC";
	const DESCENDING="DESC";

	public static function isValidOrderPolicy($order){
		return ($order == self::ASCENDING || $order == self::DESCENDING);
	}
}
?>