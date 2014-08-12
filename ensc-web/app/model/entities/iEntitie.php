<?php

namespace Entities;
interface iEntitie{
	
	public function create();
	public function delete();
	public function update();
	public static function findAll(\Data\DataProvider $dataProvider);
	public static function findByProperties(\Data\DataProvider $dataProvider, $ref, $strict, $fullProperties, $strictTextComparizon=false);
	public static function findById(\Data\DataProvider $dataProvider, $id);
}

?>