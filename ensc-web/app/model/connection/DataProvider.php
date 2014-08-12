<?php

namespace Data;
class DataProvider{

	private $dns;
	private $dbUser;
	private $dbPassword;
	private $connection;
	private $maxSelectRows=100;
	

	public function __construct(){
		
	$props = parse_ini_file("datasource.ini");
	$this->dns = "mysql:host=".$props["host"].";dbname=".$props["database"];
	$this->dbUser = $props["user"];
	$this->dbPassword = $props["password"];
	$this->connection = new \PDO($this->dns, $this->dbUser, $this->dbPassword);
	
	}

	public function setMaxSelectRows($max){
		$this->maxSelectRows=$max;
	}

	public function executeSelectQuery($q){
		$q.=" LIMIT ".$this->maxSelectRows;
		//echo $q;
		$select = $this->connection->query($q);
    	$select->setFetchMode(\PDO::FETCH_OBJ);
	    $result = $select->fetchAll(\PDO::FETCH_OBJ);
	    return $result;
	}
	
	public function executeCountQuery($q){
		$select = $this->connection->prepare($q);
		$select = $this->connection->execute($q);
		$rows = $select->fetch(\PDO::FETCH_NUM);
		return $rows[0];
	}
	
	public function executeCRUDQuery($q){
		//echo $q;
		$select = $this->connection->prepare($q);
		return $this->connection->exec($q);
	}
	
	public function getLastId(){
		return $this->connection->lastInsertId();
	}

	
	public function selectFieldsFromTable(array $tables, array $columns){
		$col_str =$this->buildElementLis($columns);
		$tbl_str=$this->buildElementLis($tables);
		$query_str = "SELECT ".$col_str." FROM ".$tbl_str;
		return $this->executeSelectQuery($query_str);
	}
	
	public function selectAllFromTable($table){

		$query_str = "SELECT * FROM ".$table;
		return $this->executeSelectQuery($query_str);
	}

	public function selectAllFromTableWithOrder($table, array $order){
		$query_str = "SELECT * FROM ".$table;
		$query_str .=" ORDER BY ";
		$query_str .= $this->buildOrderByClause($order);
		//echo $query_str;
		return $this->executeSelectQuery($query_str);
	}
	
	public function countAllEntriesFromTable($table){
		$query_str = "SELECT COUNT(*) FROM ".$table;
		return $this->executeCountQuery($query_str);
	}
	
	public function insert($table, array $elements){
		$columns = "";
		$values = "";
		$count = count($elements);
		$i=0;
		foreach($elements as $col=>$value){
			$columns = $columns.$col;
			if($value!=null){
				$values = (gettype($value)=="string")? $values."'".$value."'" : $values.$value;
			}
			else{
				$values = $values."null";
			}
			if($i!=$count-1){
				$columns = $columns.", ";
				$values = $values.", ";
			}
			$i++;
		}
		

		$query_str = "INSERT INTO ".$table." (".$columns.") VALUES (".$values.")";
		//echo $query_str;
		return $this->executeCRUDQuery($query_str);
	}
	
	

	
	public function delete($table, array $values){
		$query_str = "DELETE FROM ".$table." WHERE ".($this->buildWhereClause($values, TRUE, true, true));
		//echo $query_str;
		return $this->executeCRUDQuery($query_str);
	}

	public function update($table, array $changedValues, array $referenceValues){
		$query_str = "UPDATE ".$table." SET ";
		$i=0;
		$count = count($changedValues);
		foreach($changedValues as $column=>$value){
			$query_str = $query_str." ".$column." = ";
			if($value!=null){
				$query_str = (gettype($value)=="string")? $query_str."'".$value."'" : $query_str.$value;
			}
			else{
				$query_str = $query_str."null";
			}
			if($i!=$count-1){
				$query_str = $query_str.", ";
			}
			$i++;
		}
		$where =  $this->buildWhereClause($referenceValues, true, true, true);
		$query_str = $query_str." WHERE ".$where;
		//echo $query_str;
		return $this->executeCRUDQuery($query_str);
	}

	
	
	public function selectFieldsFromTableWithProperties(array $tables, array $columns, array $properties, $strict, $fullProperties, $formatStringValues=false, $strictStringComparizon=false){
		$col_str =$this->buildElementList($columns);
		$tbl_str=$this->buildElementList($tables);
		if(!$fullProperties){
			$properties = $this->removeNullValues($properties);
		}
		$where_str = $this->buildWhereClause($properties, $strict, $formatStringValues, $strictStringComparizon);
		$query_str = "SELECT ".$col_str." FROM ".$tbl_str." WHERE ".$where_str;
		//echo $query_str."<br/>";
		return $this->executeSelectQuery($query_str);
	}

	public function selectFieldsFromTableWithPropertiesAndOrder(array $tables, array $columns, array $properties, $strict, $fullProperties, array $order, $formatStringValues=false, $strictStringComparizon=false){
		$col_str =$this->buildElementList($columns);
		$tbl_str=$this->buildElementList($tables);
		if(!$fullProperties){
			$properties = $this->removeNullValues($properties);
		}
		$where_str = $this->buildWhereClause($properties, $strict, $formatStringValues, $strictStringComparizon);
				$order_str = $this->buildOrderByClause($order);
		$query_str = "SELECT ".$col_str." FROM ".$tbl_str." WHERE ".$where_str." ORDER BY ".$order_str;;

		return $this->executeSelectQuery($query_str);
	}
	
	public function selectDistinctFieldsFromTableWithProperties(array $tables, array $columns, array $properties, $strict, $fullProperties, $formatStringValues=false, $strictStringComparizon=false){
		$col_str =$this->buildElementList($columns);
		$tbl_str=$this->buildElementList($tables);
		if(!$fullProperties){
			$properties = $this->removeNullValues($properties);
		}
		$where_str = $this->buildWhereClause($properties, $strict, $formatStringValues, $strictStringComparizon);
		$query_str = "SELECT DISTINCT ".$col_str." FROM ".$tbl_str." WHERE ".$where_str;

		return $this->executeSelectQuery($query_str);
	}

	public function selectDistinctFieldsFromTableWithPropertiesAndOrder(array $tables, array $columns, array $properties, $strict, $fullProperties, array $order, $formatStringValues=false, $strictStringComparizon=false){
		$col_str =$this->buildElementList($columns);
		$tbl_str=$this->buildElementList($tables);
		if(!$fullProperties){
			$properties = $this->removeNullValues($properties);
		}
		$where_str = $this->buildWhereClause($properties, $strict, $formatStringValues, $strictStringComparizon);

		$order_str = $this->buildOrderByClause($order);
		$query_str = "SELECT DISTINCT ".$col_str." FROM ".$tbl_str." WHERE ".$where_str." ORDER BY ".$order_str;

		return $this->executeSelectQuery($query_str);
	}



	private static function buildElementList(array $elements){
		$list_str="";
		for($i=0;$i<count($elements);$i++){
			$list_str = $list_str.$elements[$i];
			if($i!=count($elements)-1){
				$list_str = $list_str.", ";
			}
		}
		return $list_str;
	}
	
	private static function buildWhereClause(array $values, $strict, $formatStringValues, $strictStringComparizon){
		$where ="";
		$logic = ($strict==TRUE)?" AND ":" OR ";
		$i=0;
		$count = count($values);
		foreach($values as $column=>$val){
			$where = $where.$column;
			$value=$val;
			if($val==null){
				$value=" IS NULL ";
			}
			else{
				if(!$strictStringComparizon &&  gettype($val)=="string"){
					$where = $where." LIKE ";
					$value= " '%".$val."%' ";
				}else{
					$where = $where."=";
					$value = ($formatStringValues && gettype($val)=="string")?" '".$val."' ":$val;
				}	
			}
			$where = $where.$value;
			if($i!=$count-1){
				$where = $where." ".$logic." ";
			}
			$i++;
		}
		return $where;
	}

	private static function buildOrderByClause(array $order){
		$order_str="";
		$i=0;
		$count = count($order);
		foreach($order as $column=>$policy){
			$orerPolicy="";
			if(OrderPolicy::isValidOrderPolicy($policy)){
				$orerPolicy = $policy;
			}
			else {
				$orerPolicy=OrderPolicy::ASCENDING;
			}
			$order_str = $order_str.$column." ".$policy." ";   //$policy = "ASC" or "DSC"
			if($i!=$count-1){
				$order_str = $order_str.", ";
			}
			$i++;
		}
		return $order_str;
	}
	
	private static function removeNullValues(array $values){
		foreach($values as $key=>$value){
			if($value==null){
				unset($values[$key]);
			}
		}
		return $values;
	}


	public function research(array $table, array $matchingFields, array $columns, $text){
		$query_str = "SELECT ";
		$query_str .= $this->buildElementList($columns);
		$query_str .= " FROM ".$this->buildElementList($table);
		$query_str .=" WHERE MATCH(". $this->buildElementList($matchingFields).")";
		$query_str .=" AGAINST('*".$text."*' IN BOOLEAN MODE)";
		//echo $query_str;
		return  $this->executeSelectQuery($query_str);
	}
}



?>