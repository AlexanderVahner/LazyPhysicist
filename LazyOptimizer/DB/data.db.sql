BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "Vars" (
	"LastCheckDate"	TEXT
);
CREATE TABLE IF NOT EXISTS "Objectives" (
	"PlanRowId"	INTEGER NOT NULL,
	"StructureId"	TEXT NOT NULL,
	"ObjType"	INTEGER NOT NULL,
	"Priority"	REAL NOT NULL,
	"Operator"	INTEGER NOT NULL,
	"Dose"	REAL,
	"Volume"	REAL,
	"ParameterA"	REAL
);
CREATE TABLE IF NOT EXISTS "Plans" (
	"PlanId"	TEXT NOT NULL,
	"PatientId"	TEXT NOT NULL,
	"CourseId"	TEXT NOT NULL,
	"FractionsCount"	INTEGER NOT NULL DEFAULT 0,
	"SingleDose"	NUMERIC NOT NULL,
	"MachineId"	TEXT,
	"Technique"	INTEGER,
	"SelectionFrequency"	INTEGER DEFAULT 0,
	"StructuresString"	TEXT,
	"Description"	TEXT DEFAULT ''
);
CREATE TABLE IF NOT EXISTS "NTO" (
	"PlanRowId"	INTEGER NOT NULL,
	"IsAutomatic"	INTEGER NOT NULL,
	"DistanceFromTargetBorderInMM"	REAL,
	"StartDosePercentage"	REAL,
	"EndDosePercentage"	REAL,
	"FallOff"	REAL,
	"Priority"	REAL
);
COMMIT;
