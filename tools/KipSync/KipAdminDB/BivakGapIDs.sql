ALTER TABLE dbo.kipBivak ADD
	b_GapID int NULL,
	u_GapID int NULL,
	s_GapID int NULL;
	
CREATE INDEX IDX_Bivak_BGapID ON kipBivak(B_GapID);
CREATE INDEX IDX_Bivak_UGapID ON kipBivak(U_GapID);
CREATE INDEX IDX_Bivak_SGapID ON kipBivak(S_GapID);
	