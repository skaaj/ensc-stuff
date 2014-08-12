<?php

namespace Entities;
require_once("iEntitie.php");

class Tag implements iEntitie{

	private $dataProvider;
	private static $tableName="tag";

	private $name;
	
	function __construct($name=false, $dataProvider){
		$this->dataProvider = new \Data\DataProvider();
		if($name!=false){ $this->name=$name; }
	}
	
	public function getName(){
		return $this->name;
	}

	public function create(){
		
		$elements = array("name"=>$this->name);
		$this->dataProvider->insert(self::$tableName, $elements);
	}
	
	public function delete(){
		if($this->name!=null)
		{
			$values = array("name"=>$this->name);
			$this->dataProvider->delete(self::$tableName, $values);
		}
	}
	
	public function update(){}
	
	public static function findAll(\Data\DataProvider $dataProvider){
		$datas = $dataProvider->selectAllFromTable(self::$tableName);
		$count = count($datas);
		$allFound = array();
		foreach($datas as $tuple){
			$allFound[]=new Tag($tuple->name, $dataProvider);
		}
		return $allFound;
	}
	
	public static function findByProperties(\Data\DataProvider $dataProvider, $ref, $strict, $fullProperties, $strictTextComparizon=false){
		$props =array();
		$cols = array("name");
		$props["name"]=$ref->getName();


		$matchs = $dataProvider->selectFieldsFromTableWithProperties(array(self::$tableName), $cols, $props, $strict, $fullProperties, TRUE, $strictTextComparizon);
		$matchingObjects=array();
		foreach($matchs as $tuple){
			$matchingObjects[]=new Tag($tuple->name, $dataProvider);
		}
		return $matchingObjects;
	}
	
	public static function findById(\Data\DataProvider $dataProvider, $id){
		$cols = array("name");
		$tables = array(self::$tableName);
		$tuples = $dataProvider->selectFieldsFromTableWithProperties($tables, $cols, array("name"=>$id), TRUE, TRUE, TRUE);
		$result = null;
		$tag = null; 
		if(count($tuples)>0){
			$result=$tuples[0];
			$tag=new Tag($result->name, $dataProvider);
		}
		return $tag;
	}
	
	public static function findAllWithOrder(\Data\DataProvider $dataProvider, array $order){
		$datas = $dataProvider->selectAllFromTableWithOrder(self::$tableName, $order);
		$count = count($datas);
		$allFound = array();
		foreach($datas as $tuple){
			$allFound[]=new Tag($tuple->name, $dataProvider);
		}
		return $allFound;
	}
}


?>
	