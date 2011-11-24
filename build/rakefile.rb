COMPILE_TARGET = "Release"
require "./utils.rb"

PRODUCT = "NMocha"
CLR_VERSION = 'v4.0.30319'
MSBUILD_DIR = File.join(ENV['windir'].dup, 'Microsoft.NET', 'Framework', CLR_VERSION)

NUNIT_DIR = '../lib/nunit-2.5/net-2.0/'

@nunitRunner = NUnitRunner.new :compilemode => COMPILE_TARGET, :nunit_dir => NUNIT_DIR, :source => '../src'

task :default => [:compile, :unit_test,  :acceptance_test]

task :compile  do
  include FileTest
  
  buildRunner = MSBuildRunner.new(MSBUILD_DIR)

  buildRunner.compile :compilemode => COMPILE_TARGET, :solutionfile => '../src/NMocha.sln'
Dir.mkdir 'output' unless exists?('output')

end

task :unit_test => [ :compile] do
   @nunitRunner.executeTests ['NMocha.Test']
end  


task :acceptance_test => [ :compile] do
   @nunitRunner.executeTests ['NMocha.AcceptanceTests']
end  




