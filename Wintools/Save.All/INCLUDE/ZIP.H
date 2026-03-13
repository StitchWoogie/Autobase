#define LOCAL_FILE_HEADER_SIGNATURE  0x04034b50L

typedef struct local_file_header {
	WORD version_needed_to_extract;
	WORD general_purpose_bit_flag;
	WORD compression_method;
	WORD last_mod_file_time;
	WORD last_mod_file_date;
	long crc32;
	long compressed_size;
	long uncompressed_size;
	WORD filename_length;
	WORD extra_field_length;
} local_file_header;

#define CENTRAL_FILE_HEADER_SIGNATURE  0x02014b50L

typedef struct central_directory_file_header {
	WORD version_made_by;
	WORD version_needed_to_extract;
	WORD general_purpose_bit_flag;
	WORD compression_method;
	WORD last_mod_file_time;
	WORD last_mod_file_date;
	long crc32;
	long compressed_size;
	long uncompressed_size;
	WORD filename_length;
	WORD extra_field_length;
	WORD file_comment_length;
	WORD disk_number_start;
	WORD internal_file_attributes;
	long external_file_attributes;
	long relative_offset_local_header;
} central_directory_file_header;

#define END_CENTRAL_DIR_SIGNATURE  0x06054b50L

typedef struct end_central_dir_record {
	WORD number_this_disk;
	WORD number_disk_with_start_central_directory;
	WORD total_entries_central_dir_on_this_disk;
	WORD total_entries_central_dir;
	long size_central_directory;
	long offset_start_central_directory;
	WORD zipfile_comment_length;
} end_central_dir_record;
