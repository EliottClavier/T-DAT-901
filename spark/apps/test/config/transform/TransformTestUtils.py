import os


class TransformTestUtils:

    @staticmethod
    def clean_input_directory(path):
        if not os.path.exists(path):
            return
        for rep, sreps, fics in os.walk(path, topdown=False):
            for fic in fics:
                os.remove(os.path.join(rep, fic))
            for srep in sreps:
                os.rmdir(os.path.join(rep, srep))
        os.rmdir(path)

    @staticmethod
    def clean_test_directories(directories):
        for directory in directories:
            TransformTestUtils.clean_input_directory(directory)

    @staticmethod
    def convert_file_to_parquet(spark, file_path, parquet_path, file_format="json"):
        format_df = spark \
            .read \
            .format(file_format) \
            .load(file_path)

        if format_df.count() == 0:
            return

        format_df.write \
            .mode("overwrite") \
            .parquet(parquet_path)

    @staticmethod
    def convert_tests_files_to_parquet(spark, test_root_path, file_format, tests_names, file_paths_names):
        for test_name, title in tests_names:
            for file_path_name in file_paths_names:
                root_path = f'{test_root_path}/{test_name}'
                file_path = f'{root_path}/{file_path_name}.{file_format}'
                file_path_name = "parquet" if "input" in file_path_name else file_path_name
                parquet_path = f'{root_path}/{file_path_name}/'
                TransformTestUtils.convert_file_to_parquet(spark,
                                                           file_path,
                                                           parquet_path)

    @staticmethod
    def get_tests_path_to_clean(root_path, tests_names, file_names):
        tests_path_to_clean = []
        for test_name, title in tests_names:
            tests_path_to_clean.append(f'{root_path}/{test_name}/parquet')
            for file_name in file_names:
                tests_path_to_clean.append(f'{root_path}/{test_name}/{file_name}')
        return tests_path_to_clean
