﻿CREATE TABLE REGISTER (
	DOCNO TEXT,
	TITLE TEXT,
	ORGANIZATION TEXT,
	REVISION TEXT,
	REVISION_DATE TEXT,
	DOC_VERSION NUMBER,
	REGISTERED_BY TEXT,
	REGISTERED TEXT,
	INT_CD TEXT,
	DESCR MEDIUMTEXT,
	DISCIPLINE TEXT,
	REVIEW_STATUS TEXT,
	DOC_STATUS TEXT,
	DOC_TYPE TEXT,
	ATTRIB1 TEXT,
	ATTRIB2 TEXT,
	ATTRIB3 TEXT,
	ATTRIB4 TEXT,
	DOC_CURRENT TEXT,
	INTERNAL_CODES TEXT
);

CREATE TABLE REVIEW_INFO ( 
	DOCNO TEXT,
	DOC_VERSION TEXT,
	REVIEW_DATE TEXT,
	REVIEW_STATUS TEXT,
	REVIEW_NOTE TEXT,
	REVIEWED_BY TEXT
);

CREATE TABLE CLSS ( 
	LEVEL TEXT,
	CODE TEXT,
	NAME TEXT
);

CREATE TABLE ARCHIVE (
	ID TEXT,
	ARCHIVE_TYPE TEXT,
	DESCRIPTION TEXT,
	FILE_SEQ TEXT,
	METADATA MEDIUMTEXT,
	CREATED TEXT
);