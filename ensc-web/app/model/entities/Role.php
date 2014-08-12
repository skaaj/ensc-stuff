<?php
namespace Entities;
require_once("iEntitie.php");

class Role implements iEntitie{
	private $dataProvider;
	private static $tableName="role";
	private $id;
	private $name;

	
	function __construct($id=null, $name=null, $dataProvider){
		$this->dataProvider = new \Data\DataProvider();
		
		if($id!=null){ $this->id=$id; }
		
		if($name!=null){ $this->name=$name; }
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
			$allFound[]=new Role($role->id, $role->name, $dataProvider);
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
			$matchingObjects[]=new Role($tuple->id, $tuple->name, $dataProvider);
		}
		return $matchingObjects;
	}
	
		
	public static function findById(\Data\DataProvider $dataProvider, $id){
		$cols = array("id", "name");
		$tables=array(self::$tableName);
		$tuples = $dataProvider->selectFieldsFromTableWithProperties($tables, $cols, array("id"=>$id), TRUE, TRUE, TRUE);
		$result = null;
		$role = null;
		if(count($tuples)>0){
			$result=$tuples[0];
			$role=new Role($result->id, $result->name, $dataProvider);
		}
		return $role;
	}
	
	public function toValueObject(){
		return new \ValueObjects\RoleVO($this->id, $this->name);
	}
}



?>