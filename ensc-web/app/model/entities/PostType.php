<?php
namespace Entities;
require_once("iEntitie.php");

class PostType implements iEntitie{
	private $dataProvider;
	private static $tableName="post_type";
	private $id;
	private $name;

	
	function __construct($id=false, $name=false, $dataProvider){
		$this->dataProvider = new \Data\DataProvider();
		
		if($id!=false){ $this->id=$id; }
		
		if($name!=false){ $this->name=$name; }
	}
	
	public function getName(){
		return $this->name;
	}
	public function getId(){
		return $this->id;
	}
	
	public function create(){
		$elements = array("name"=>$this->name);
		$this->dataProvider->insert(self::$tableName, $elements);
	}
	
	public function delete(){
		$values = array("name"=>$this->name);
		$this->dataProvider->delete(self::$tableName, $values);
	}
	
	public function update(){}
	
	public static function findAll(\Data\DataProvider $dataProvider){
		$datas = $dataProvider->selectAllFromTable(self::$tableName);
		$count = count($datas);
		$allFound = array();
		foreach($datas as $role){
			$allFound[]=new PostType($role->id, $role->name, $dataProvider);
		}
		return $allFound;
	}
	
	public static function findByProperties(\Data\DataProvider $dataProvider, $ref, $strict, $fullProperties, $strictTextComparizon=false){
		$props =array();
		$cols = array("id", "name");
		$props["id"] = $ref->getId();
		$props["name"]=$ref->getName();
		$matchs = $dataProvider->selectFieldsFromTableWithProperties(array(self::$tableName), $cols, $props, $strict, $fullProperties, TRUE, $strictTextComparizon);
		$matchingObjects=array();
		foreach($matchs as $tuple){
			$matchingObjects[]=new PostType($tuple->id, $tuple->name, $dataProvider);
		}
		
		return $matchingObjects;
	}
	
	public static function findById(\Data\DataProvider $dataProvider, $id){
		$cols = array("id", "name");
		$tables = array(self::$tableName);
		$tuples = $dataProvider->selectFieldsFromTableWithProperties($tables, $cols, array("id"=>$id), TRUE, TRUE, TRUE, TRUE);
		$result = null;
		$type = null;
		if(count($tuples)>0){
			$result=$tuples[0];
			$type=new PostType($result->id, $result->name, $dataProvider);
		}
		return $type;
	}

			
		
}
?>
	