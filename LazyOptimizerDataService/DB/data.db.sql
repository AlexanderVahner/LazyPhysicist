BEGIN TRANSACTION;
PRAGMA user_version = 1;
CREATE TABLE IF NOT EXISTS "Vars" (
	"LastCheckDate"	TEXT
);
CREATE TABLE IF NOT EXISTS "Ntos" (
	"PlanRowId"	INTEGER NOT NULL,
	"IsAutomatic"	INTEGER NOT NULL,
	"DistanceFromTargetBorderInMM"	REAL,
	"StartDosePercentage"	REAL,
	"EndDosePercentage"	REAL,
	"FallOff"	REAL,
	"Priority"	REAL
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
	"CreationDate"	TEXT,
	"MachineId"	TEXT,
	"Technique"	TEXT,
	"ApprovalStatus"	INTEGER,
	"Starred"	INTEGER,
	"SelectionFrequency"	INTEGER DEFAULT 0,
	"StructuresString"	TEXT,
	"Description"	TEXT DEFAULT ''
);
CREATE INDEX IF NOT EXISTS "IdxNTO_PlanRowId" ON "Ntos" (
	"PlanRowId"	ASC
);
CREATE INDEX IF NOT EXISTS "IdxObjectives_PlanRowId" ON "Objectives" (
	"PlanRowId"	ASC
);
CREATE INDEX IF NOT EXISTS "IdxPlans_CreationDate" ON "Plans" (
	"CreationDate"	ASC
);
COMMIT;
